'use strict';
IteroJS.baseUrl = "http://itero.demo.pactas.com/api/v1/";
var providerReturnUrl = "http://<yourdomain>/finalize.html";

/// The modal for the 3D-Secure popup needs a simple controller to pass along some data.
var ChangePaymentMethodController = function ($scope, $modalInstance, token, iteroJS, onClose) {
    var self = this;
    $scope.paymentData = {};
    $scope.paymentMethods = iteroJS.paymentMethods;
    $scope.paymentMethodEnum = iteroJS.paymentMethodEnum;

    // A lookup for friendly strings of the payment methods
    $scope.paymentMethodNames = {
        "CreditCard:Paymill": "Credit Card", "Debit:Paymill": "Direct Debit", "Skrill": "Skrill",
        "PayPal": "PayPal", "CreditCard:PayOne": "Credit Card", "Debit:PayOne": "Direct Debit",
        "CreditCard:FakeProvider": "Credit Card", "Debit:FakeProvider": "Direct Debit", "FakePSP" : "Fake Provider",
        "None:None": "None", "InvoicePayment" : "Invoice"
    };

    $scope.proceed = function () {
        $scope.paymentData.returnUrl = providerReturnUrl;
        // change the payment method by sending the new payment information to iteroJS. This might
        // contain the actual credit card information, so don't log that information. IteroJS will
        // call the provider via javascript and exchange the data for a token or fake data.
        iteroJS.changePaymentMethod(token, $scope.paymentData, function (data) {
            debug.log("changePaymentMethod returned", data);
            $modalInstance.close('done');
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
        iteroJS.cancelContract(function (data) {
            $modalInstance.close('done');
        }, function (error) {
            $modalInstance.dismiss('error: ' + error.Message);
        });
    };
};

var ChangePlanController = function ($scope, $modalInstance, $http, productInfo) {
    $scope.close = function () { $modalInstance.dismiss('cancel'); }
    $scope.productInfo = productInfo;
    $scope.dto = { targetPlanVariantId : "" };
    $scope.changePlan = function () {
        if (!$scope.dto.targetPlanVariantId)
            return;
        $http.post("/upgrade", $scope.dto).success(function (data) {
            var iteroUpgrade = new IteroJS.Upgrade();
            var orderId = data.Id;
            // Trigger a synchronous payment at pactas. The callback will be invoked once the payment has finished
            iteroUpgrade.payUpgradeSync(null, orderId, function (data) {
                $modalInstance.close('done');
            }, function (data) {
                console.log("Upgrade failed!", data);
            });
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

    var config = {
        // REQUIRED. Id of the calling entity
        // TODO: Use a key instead that can be changed
        "entityId": "",

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
                token: function () { return config.token; }
            }
        });

        modalInstance.result.then(function (result) {
            loadContract();
        }, function () {
            // TODO: Error Handling
        });
    }

    $scope.changePlanDialog = function () {
        var modalInstance = $modal.open({
            templateUrl: 'change-plan-dialog.html',
            controller: ChangePlanController,
            windowClass: "fade in",
            resolve: {
                productInfo : function() { return $scope.plan.Products; },
                token: function () { return config.token; }
            }
        });

        modalInstance.result.then(function (result) {
            loadContract();
        }, function () {
            // TODO: Error Handling
        });
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
        }, function () {
            // TODO: Error Handling
        });
    }

    $scope.changePaymentMethod = function () {
        var iteroJSCPM = new IteroJS.ChangePaymentMethod(config, function () {
            var modalInstance = $modal.open({
                templateUrl: 'change-payment-method.html',
                controller: ChangePaymentMethodController,
                windowClass: "fade in",
                resolve: {
                    onClose: function () {
                        return function () { };
                    },
                    //iteroJS: function () { return iteroJSCPM; },
                    token: function() { return config.token; }
                }
            });

            modalInstance.result.then(function (result) {
                loadContract();
            }, function () {
                // TODO: Error Handling
            });
        });
    }

    $scope.downloadUrl = function (invoiceId) {
        // Return a download URL based on the invoice id. This is basically a string concatenation helper
        return self.iteroInstance.downloadUrl(invoiceId);
    };

    function loadContract() {
        self.iteroInstance.getContractDetails(function (data) {
            $scope.$apply(function () {
                $scope.plan = data;
            });
        });
    }

    $http({ method: "GET", url: "/config", cache: false }).success(function (data) {
        console.log("nantero config loaded", data);
        config.entityId = data.EntityId;
        config.baseUrl = data.IteroBaseUrl;
        config.token = data.Token;

        self.iteroInstance = new IteroJS.Portal(config);
        loadContract();
    });  
};

// angularjs dependency injection
angular.module('iteroAngular', ['ui.bootstrap.modal']);
