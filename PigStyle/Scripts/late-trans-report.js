﻿var LateTransApp = angular.module('LateTransApp', ['ngAnimate', 'ui.bootstrap', 'darthwade.loading']);

LateTransApp.controller('LTCtrl', function ($scope, $http, $filter, $loading, $uibModal) {
    $scope.dateOptions = {
        showWeeks: false,
        minDate: new Date(2006, 0, 25),
    };

    $scope.altInputFormats = ['M!/d!/yyyy']

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

    $scope.selectedOption = 'init';
    $scope.selectedDept = 'all';

    $scope.verifyDates = function () {
        var dateMin = new Date(2006, 0, 25);

        if ($scope.fromDate.dt < dateMin) {
            alert("From Date is earlier than the minimum date allowed.\nChanging From Date to minimum date...");
            $scope.fromDate.dt = dateMin;
        };

        if ($scope.toDate.dt < $scope.fromDate.dt) {
            alert("To Date is earlier than From Date. Please change.");
            return false;
        };

        if ($scope.toDate.dt > Date.now()) {
            alert("To Date is later than current date.\nChanging the To Date to current date.");
            $scope.toDate.dt = Date.now();
        };

        return true;
    };

    $scope.showReport = function () {
        if (!$scope.verifyDates())
            return;

        if ($scope.selectedOption === 'init' || $scope.selectedOption === '') {
            alert("That is not a valid Store/Area selection.");
            return;
        };

        if ($scope.selectedDept === 'init' || $scope.selectedDept === '') {
            alert("That is not a valid Department selection.");
            return;
        };

        $loading.start('modalOpen');
        var fromDate = $filter('date')($scope.fromDate.dt, 'MM/dd/yyyy');
        var toDate = $filter('date')($scope.toDate.dt, 'MM/dd/yyyy');

        $http.post('LateTransmissionReport.aspx/GetPeriodData', angular.toJson({
            fromDate: fromDate,
            toDate: toDate,
            selectedOption: $scope.selectedOption,
            selectedDept: $scope.selectedDept
        })).
        then(function (response) {
            if (response.data.d == 'false') {
                $loading.finish('modalOpen');
                alert("There was no information found.");
            } else {
                var lateTransInfos = JSON.parse(response.data.d);

                lateTransInfos.fromDate = fromDate;
                lateTransInfos.toDate = toDate;
                lateTransInfos.criteria = $scope.selectedOption;
                lateTransInfos.dept = $scope.selectedDept;

                var ltrInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'latetrans-template.html',
                    controller: 'ltrInstanceCtrl',
                    size: 'lg',
                    resolve: {
                        lateTransInfos: function () {
                            return lateTransInfos;
                        },
                    },
                });

                ltrInstance.rendered.then(function () {
                    $loading.finish('modalOpen');
                });
            }
        }, function (response) {
            alert("Status Code " + response.status + ": " + response.statusText + "\n" + response.data.d);
            $loading.finish('modalOpen');
        });
    };
});

LateTransApp.controller('ltrInstanceCtrl', function ($scope, $uibModalInstance, $http, $anchorScroll, lateTransInfos) {
    $scope.lateTransInfos = lateTransInfos;

    $scope.elementId = "init"

    $scope.close = function () {
        $uibModalInstance.dismiss();
    };

    $scope.jumpTo = function () {
        $anchorScroll($scope.elementId);
    };

    $scope.requestFile = function () {
        $http.post('LateTransmissionReport.aspx/EmailReport', angular.toJson({
            fromDate: lateTransInfos.fromDate,
            toDate: lateTransInfos.toDate,
            criteria: lateTransInfos.criteria,
            dept: lateTransInfos.dept
        })).
        then(function (response) {
            alert(response.data.d);
        }, function (response) {
            alert(response.data.d);
        });
    };
});