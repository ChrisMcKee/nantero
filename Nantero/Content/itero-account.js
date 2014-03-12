'use strict';

function errorHandler(errorData) {
    console.log("An error occurred!", errorData);
}

/// The modal for the 3D-Secure popup needs a simple controller to pass along some data.
var ChangePaymentMethodController = function ($scope, $modalInstance, token, iteroJS, iteroJSPayment , onClose) {
    var self = this;
    $scope.paymentData = {};
    $scope.paymentMethods = iteroJSPayment.getAvailablePaymentMethods();
    $scope.paymentMethodEnum = iteroJSPayment.getAvailablePaymentMethodEnum();

    // A lookup for friendly strings of the payment methods
    $scope.paymentMethodNames = {
        "CreditCard:Paymill": "Credit Card", "Debit:Paymill": "Direct Debit", "Skrill": "Skrill",
        "PayPal": "PayPal", "CreditCard:PayOne": "Credit Card", "Debit:PayOne": "Direct Debit",
        "CreditCard:FakeProvider": "Credit Card", "Debit:FakeProvider": "Direct Debit", "FakePSP" : "Fake Provider",
        "None:None": "None", "InvoicePayment" : "Invoice"
    };

    $scope.proceed = function () {
        // change the payment method by sending the new payment information to iteroJS. This might
        // contain the actual credit card information, so don't log that information. IteroJS will
        // call the provider via javascript and exchange the data for a token or fake data.
        iteroJS.paymentChange(iteroJSPayment, $scope.paymentData, function (data) {
            debug.log("changePaymentMethod returned", data);
            $modalInstance.close('done');
        }, function (error) {
            console.log("an error occurred!", error);
        });
    }

    $scope.close = function () { $modalInstance.dismiss('close'); onClose(); }
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
        onClose();
    };
};

var CancelController = function ($scope, $modalInstance, contract, iteroJS) {
    $scope.close = function () { $modalInstance.dismiss('cancel'); }
    $scope.contract = contract;

    $scope.cancelContract = function () {
        iteroJS.contractCancel(function (data) {
            $modalInstance.close('done');
        }, function (error) {
            $modalInstance.dismiss('error: ' + error.Message);
        });
    };
};

var ChangePlanController = function ($scope, $modalInstance, $http, productInfo, iteroJS) {
    $scope.close = function () { $modalInstance.dismiss('cancel'); }
    $scope.productInfo = productInfo;
    $scope.dto = { targetPlanVariantId : "" };
    $scope.changePlan = function () {
        if (!$scope.dto.targetPlanVariantId)
            return;
        $http.post("/upgrade", $scope.dto).success(function (data) {
            var orderId = data.Id;
            // Trigger a synchronous payment at pactas. The callback will be invoked once the payment has finished
            iteroJS.upgradePaySync(orderId, function (data) {
                $modalInstance.close('done');
            }, errorHandler);
        });
    }
}

var ChangeCustomerDataController = function ($scope, $modalInstance, customer, iteroJS) {
    $scope.customer = customer;
    $scope.proceed = function () {
        iteroJS.changeCustomerData(customer,
            function (data) {
                debug.log("changeCustomerData returned", data);
                $modalInstance.close('done');
            }
        );
    }
    $scope.cancel = function () { $modalInstance.dismiss('cancel'); }
}

var AccountController = function ($scope, $http, $modal) {
    var self = this;
    var customerToken = null;

    var paymentConfig = {
        // REQUIRED. An API key to associate your call.
        "publicApiKey": "",

        // OPTIONAL for Paymill and PayOne, otherwise REQUIRED. Specifies the redirect URL for PSPs like PayPal, Skrill, ...
        // FIXME: Nantero should get this from config
        "providerReturnUrl": "http://<yourdomain>/finalize.html",

        // OPTIONAL. Overwrite the handling of the 3d-secure iframes. Comment out these 
        // two lines to see what happens without (essentially the same, but not customizable).
        // Only applies to paymill. You might want to read paymill's documentation on the subject.
        //"popupCreate": tdsInit,
        //"popupClose": tdsCleanup
    };

    $scope.cancelPlanDialog = function () {
        var modalInstance = $modal.open({
            templateUrl: 'cancel-dialog.html',
            controller: CancelController,
            windowClass: "fade in",
            resolve: {
                contract: function () { return $scope.plan; },
                iteroJS: function () { return self.iteroInstance; },
                token: function () { return customerToken; }
            }
        });

        modalInstance.result.then(function (result) {
            loadContract();
        }, errorHandler);
    }

    $scope.changePlanDialog = function () {
        var modalInstance = $modal.open({
            templateUrl: 'change-plan-dialog.html',
            controller: ChangePlanController,
            windowClass: "fade in",
            resolve: {
                productInfo : function() { return $scope.plan.Products; },
                token: function () { return customerToken; },
                iteroJS : function() { return self.iteroInstance; }
            }
        });

        modalInstance.result.then(function (result) {
            loadContract();
        }, errorHandler);
    }

    $scope.changeCustomerData = function () {
        var modalInstance = $modal.open({
            templateUrl: 'change-customer-data.html',
            controller: ChangeCustomerDataController,
            windowClass: "fade in",
            resolve: {
                customer: function () { return $scope.plan.Customer; },
                iteroJS: function () { return self.iteroInstance; }
            }
        });

        modalInstance.result.then(function (result) {
            loadContract();
        }, errorHandler);
    }

    $scope.changePaymentMethod = function () {
        var iteroJSPayment = new IteroJS.Payment(paymentConfig, function() {
            // when the payment has loaded, show the modal
            // TBD: wouldn't it be better to show the modal with a loading indicator?
            var modalInstance = $modal.open({
                templateUrl: 'change-payment-method.html',
                controller: ChangePaymentMethodController,
                windowClass: "fade in",
                resolve: {
                    onClose: function () {
                        return function () { };
                    },
                    iteroJS: function () { return self.iteroInstance; },
                    iteroJSPayment : function( ) { return iteroJSPayment; },
                    token: function () { return customerToken; }
                }
            });

            modalInstance.result.then(function (result) {
                loadContract();
            }, errorHandler);
        }, errorHandler);
    }

    $scope.downloadUrl = function (invoiceId) {
        // Return a download URL based on the invoice id. This is basically a string concatenation helper
        return self.iteroInstance.invoicePdfDownloadUrl(invoiceId);
    };

    function loadContract() {
        self.iteroInstance.contractDetails(function (data) {
            $scope.$apply(function () {
                $scope.plan = data;
            });
        }, errorHandler);
    }

    $http({ method: "GET", url: "/config", cache: false }).success(function (data) {
        console.log("nantero config loaded", data);
        paymentConfig.publicApiKey = data.EntityId;
        customerToken = data.Token;

        self.iteroInstance = new IteroJS.Portal(customerToken);
        loadContract();
    });  
};

// angularjs dependency injection
angular.module('iteroAngular', ['ui.bootstrap.modal']);
