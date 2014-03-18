'use strict';

/// The modal for the 3D-Secure popup needs a simple controller to pass along some data.
var ModalInstanceCtrl = function ($scope, $modalInstance, url, params, onclose) {
    $scope.url = url;
    $scope.params = params;
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
        onclose();
    };
};

var SignupController = function ($scope, $http, $modal) {
    var self = this;
    $scope.order = null;
    // Some default data so we don't have to enter a ton of info every time
    $scope.customerData = { firstName: "Marcellus", lastName: "Wallace", email: "mw@example.com" };
    $scope.paymentData = { bearer: "CreditCard:Paymill", "cardNumber": "5169147129584558", cardHolder: "Marcellus Wallace", cvc: "911", expiryMonth: "12", expiryYear: "2015" };
    $scope.paymentMethods = {};
    $scope.paymentMethodEnum = [];
    $scope.paymentReady = false;

    // A lookup table for 'friendly' payment provider names
    $scope.paymentMethodNames = {
        "CreditCard:Paymill": "Credit Card", "Debit:Paymill": "Direct Debit", "Skrill": "Skrill",
        "PayPal": "PayPal", "CreditCard:PayOne": "Credit Card", "Debit:PayOne": "Direct Debit",
        "CreditCard:FakeProvider": "Credit Card", "Debit:FakeProvider": "Direct Debit", "FakePSP" : "Fake Provider",
        "None:None": "None", "InvoicePayment" : "Invoice"
    };

    // The signup method that is called when the user submits the form
    $scope.signUp = function () {
        // To indicate that the site is working and to disable the signup button, we're setting a flag
        $scope.signupRunning = true;

        // This is an example of how to customize the signup behavior: Instead of sending the signup to pactas directly, we'll
        // make a call to our nantero server first and try to register the user with his selected subdomain (in a real application,
        // you'd want to make sure it's unique). Once the user is registered in nantero (and has an Id there), we'll proceed with 
        // the signup on the pactas side. That requires us...
        $http({ method: "POST", url: "/register", data: { "Subdomain": $scope.customerData.subdomain }, cache: false }).success(function (nanteroResponse) {
            // ... to be able to map the newly created user in nantero to the user in the pactas signup process. Let's make sure our user was created
            // successfully and has an id:
            if (!nanteroResponse["Id"])
                return; // error.

            // Now, assign our newly created object's id as itero's "Tag" for the customer so we later know who is who:
            console.log("Assigning tag: " + nanteroResponse.Id);
            $scope.customerData.Tag = nanteroResponse.Id;

            // And here goes the actual call to Pactas.Itero:
            self.iteroInstance.subscribe(self.iteroJSPayment, $scope.order, $scope.customerData, $scope.paymentData, function (data) {
                // This callback will be invoked when the signup succeeded (or failed)
                $scope.$apply(function () {
                    // must use $apply, otherwise angularjs won't notice we're changing the $scope's state
                    $scope.signupRunning = false;
                    if (data.Success) {
                        if (!data.Success.Url)
                            // done - we're finished and the payment has succeeded. We could notify nantero that the signup 
                            // has completed from here, but that would be dangerous because this code is public and not reliable. 
                            // So we'll wait for the webhook in the backend. Also...
                            $scope.isSuccess = true;
                        else {
                            // ... we might have to redirect the user to Skrill or PayPal, in which case the payment hasn't
                            // really completed yet. So let's perform the redirect:
                            window.location = data.Success.Url;
                            // If we got into this branch, we're giving up flow control. The user will hopefully come back
                            // to finalize.html
                        }
                    }
                });
            }, function (error) {
                // TODO: Error handling! 
                debug.error("error: ", error);
                $scope.isError = true;
            });
        });
    };

    $scope.preview = function () {
        // ask IteroJS to update the expected total. preview() will internally use a timeout so it doesn't
        // send a ton of requests and we don't need to bother with timeouts here:
        self.iteroInstance.preview($scope.order, $scope.customerData, function (data) {
            // use $scope.$apply so angular knows we're messing around with the scope's state again
            $scope.$apply(function () {
                // just copy the order from the response to the scope. You can use an inspector or the 'developer'
                // checkbox to show the whole thing:
                $scope.order = data.Order;
            });
        }, function (error) { console.log("error in preview!", error); });
    };

    var config = {
        // REQUIRED. Id of the calling entity
        // This will probably change to an API key in the final version
        "entityId": "",

        //REQUIRED. Specifies the redirect URL for PSPs like PayPal, Skrill, ...
        "providerReturnUrl" : "http://<yourdomain>/finalize.html",
    };

    // Load the configuration from the nantero server. In a real application, you could also hard-code that information, 
    // but keeping the data in one central location can't hurt:
    $http({ method: "GET", url: "/config", cache: false }).success(function (data) {
        console.log("nantero config loaded", data);
        config.publicApiKey = data.EntityId;
        config.baseUrl = data.IteroBaseUrl;

        var paymentConfig = config;
        self.iteroInstance = new IteroJS.Signup();

        var cart = { planVariantId: data.InitialPlanVariantId };
        //self.iteroInstance.preview(cart, customer, succes, error);
        self.iteroInstance.preview(cart, {}, function (data) {
            console.log("preview", data);
            $scope.$apply(function () {
                $scope.order = data.Order;
            });

        }, function (error) { });

        self.iteroJSPayment = new IteroJS.Payment(paymentConfig, function () {
            console.log("Payment loaded successfully!");
            $scope.$apply(function () {
                // When IteroJS is ready, copy the payment methods and initial order
                $scope.paymentReady = true;
                $scope.paymentMethods = self.iteroJSPayment.getAvailablePaymentMethods();
                $scope.paymentMethodEnum = self.iteroJSPayment.getAvailablePaymentMethodEnum();
                $scope.paymentData.bearer = $scope.paymentMethodEnum[0];
                
                // FIXME: THAT'S REALLY MESSY!
                //if ($scope.order.AllowWithoutPaymentData) {
                //    $scope.paymentMethods.None = {};
                //    $scope.paymentMethodEnum.push("None:None");
                //}
            });
        }, function (error) { console.log("error loading the payment", error); });
    });
};

// angularjs dependency injection
angular.module('iteroAngular', ['ui.bootstrap.modal']);
