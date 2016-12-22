angular.module('billingApp', ['ngAnimate', 'ui.bootstrap']);

angular.module('billingApp').controller('StoreBillingCtrl', function ($scope, $uibModal) {
    $scope.openStoreBilling = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'storeBillingContent.html',
            controller: 'ModalInstanceCtrl',
        });
    };
});

angular.module('billingApp').controller('OfficeBillingCtrl', function ($scope, $uibModal) {
    $scope.openOfficeBilling = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'officeBillingContent.html',
            controller: 'ModalInstanceCtrl',
        });
    };
});

angular.module('billingApp').controller('ModalInstanceCtrl', function ($scope, $uibModalInstance) {
    $scope.close = function () {
        $uibModalInstance.dismiss();
    }
});