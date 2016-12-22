angular.module('hansenApp', ['ngAnimate', 'ui.bootstrap']);

angular.module('hansenApp').controller('DatepickerCtrl', function ($scope) {
    $scope.startDate = new Date();
    $scope.endDate = new Date();

    $scope.status = {
        startOpened: false,
        endOpened: false,
    };

    $scope.openStart = function ($event) {
        $scope.status.startOpened = true;
    };

    $scope.openEnd = function ($event) {
        $scope.status.endOpened = true;
    };

    $scope.$watch('startDate', function (dateVal) {
        var oldDate = new Date(dateVal);
        var tempDate = getStartDateOfWeek(dateVal);

        if (oldDate.getTime() != tempDate.getTime()) {
            $scope.startDate = new Date(tempDate);
        }
    });

    $scope.$watch('endDate', function (dateVal) {
        var oldDate = new Date(dateVal);
        var tempDate = getEndDateOfWeek(dateVal);

        if (oldDate.getTime() != tempDate.getTime()) {
            $scope.endDate = new Date(tempDate);
        }
    });

    function getStartDateOfWeek(date) {
        var ISOweekStart = date;
        ISOweekStart.setDate(date.getDate() - date.getDay());
        return ISOweekStart;
    }

    function getEndDateOfWeek(date) {
        var ISOweekEnd = date;
        ISOweekEnd.setDate(date.getDate() + (6 - date.getDay()));
        return ISOweekEnd;
    }
});

angular.module('hansenApp').controller('ReportCtrl', function ($scope) {
    $scope.aggregateTblData = {};
    $scope.yearlyTblData = {};

    $scope.$watch('aggregateData', function (reportData) {
        if (document.getElementById('aggregateData') != null) {
            var data = JSON.parse(document.getElementById('aggregateData').value);
            $scope.aggregateTblData = data;
        }

        $scope.aggregateCollection = [].concat($scope.aggregateTblData);
    });

    $scope.$watch('yearlyData', function (reportData) {
        if (document.getElementById('yearlyData') != null) {
            var data = JSON.parse(document.getElementById('yearlyData').value);
            $scope.yearlyTblData = data;
        }

        $scope.yearlyCollection = [].concat($scope.yearlyTblData);
    });
});