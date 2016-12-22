var LateTransSum = angular.module('LateSumApp', ['ngAnimate', 'ui.bootstrap', 'darthwade.loading']);

LateTransSum.controller('LSCtrl', function ($scope, $http, $filter, $loading, $uibModal) {
    $scope.dateOptions = {
        showWeeks: false,
        minDate: new Date(2006, 0, 25),
    };

    $scope.altInputFormats = ['M!/d!/yyyy'];

    $scope.fromDate = {
        dt: Date.now(),
        opened: false,
    };

    $scope.toDate = {
        dt: Date.now(),
        opened: false,
    };

    $scope.openFrom = function () {
        $scope.fromDate.opened = true;
    };

    $scope.openTo = function () {
        $scope.toDate.opened = true;
    };

    $scope.perTarget = 0;
    $scope.selectedAreas = ['all'];

    $scope.clearDist = function () {
        $scope.selectedAreas = ['all'];
    };

    $scope.verifyDates = function () {
        var dateMin = new Date(2006, 0, 25);

        if ($scope.fromDate.dt < dateMin) {
            alert("From Date is earlier than the minimum date allowed.\nChanging From Date to minimum date.");
            $scope.fromDate.dt = dateMin;
        };

        if ($scope.toDate.dt < $scope.fromDate.dt) {
            alert("To Date is earlier than From Date. Please change.");
            return false;
        };

        if ($scope.toDate > Date.now()) {
            alert("To Date is later than current date.\nChanging To Date to current date.");
            $scope.toDate.dt = Date.now();
        };

        return true;
    };

    $scope.showReport = function () {
        if (!$scope.verifyDates())
            return;

        if ($scope.selectedAreas.includes("")) {
            alert("There is an invalid Area selection.");
            return;
        };

        $loading.start('modalOpen');
        var fromDate = $filter('date')($scope.fromDate.dt, 'MM/dd/yyyy');
        var toDate = $filter('date')($scope.toDate.dt, 'MM/dd/yyyy');

        $http.post('LateTransmissionSummary.aspx/GetPeriodData', angular.toJson({
            fromDate: fromDate,
            toDate: toDate,
            perTarget: $scope.perTarget,
            selectedAreas: $scope.selectedAreas,
        })).
        then(function (response) {
            $loading.finish('modalOpen');
        }, function (response) {
            alert("Status Code " + response.status + ": " + response.statusText + "\n" + response.data.d);
            $loading.finish('modalOpen');
        });
    };
});