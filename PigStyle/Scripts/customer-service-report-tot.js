var CSRTApp = angular.module('CSRepTotApp', ['ngAnimate', 'ui.bootstrap', 'darthwade.loading']);

CSRTApp.controller('CSRTCtrl', function ($scope, $http, $filter, $loading, $uibModal) {

    $scope.dateOptions = {
        showWeeks: false,
        minDate: new Date(2006, 0, 25)
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

    $scope.selectedOptions = ['all'];

    $scope.verifyDates = function () {
        var dateMin = new Date(2006, 0, 25);

        if ($scope.fromDate.dt < dateMin) {
            alert('From Date is earlier than the minimum date allowed.\nChanging from Date to minimum date...');
            $scope.fromDate.dt = dateMin;
        };

        if ($scope.toDate.dt < $scope.fromDate.dt) {
            alert('To Date is earlier than From Date. Please change.')
            return false;
        };

        if ($scope.toDate.dt > Date.now()) {
            alert('To Date is later than current date.\nChanging the To Date to current date.');
            $scope.toDate.dt = Date.now();
        };

        return true;
    };

    $scope.showReport = function () {
        if (!$scope.verifyDates())
            return;

        $loading.start('modalOpen');
        var fromDate = $filter('date')($scope.fromDate.dt, 'MM/dd/yyyy');
        var toDate = $filter('date')($scope.toDate.dt, 'MM/dd/yyyy');

        $http.post('CustomerServiceReportTotals.aspx/GetPeriodData', angular.toJson({ fromDate: fromDate, toDate: toDate, selectedOptions: $scope.selectedOptions})).
        then(function (response) {
            var totalInfo = JSON.parse(response.data.d);
            if (totalInfo === null) {
                $loading.finish('modalOpen');
                alert("There was no information found.");
            } else {
                $scope.collateData(totalInfo);

                totalInfo.fromDate = fromDate;
                totalInfo.toDate = toDate;

                var csrInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'csr-template.html',
                    controller: 'csrTotalCtrl',
                    size: 'lg',
                    resolve: {
                        totalInfo: function () {
                            return totalInfo;
                        }
                    }
                });

                csrInstance.rendered.then(function () {
                    $loading.finish('modalOpen');
                });
            }
        });
    };

    $scope.collateData = function (totalInfo) {
        var transTtl = {
            numTrans: 0,
            numLate: 0,
            avgPerc: 0,
        };
        var delivTtl = {
            numDeliv: 0,
            caseTtl: 0,
            avgCase: 0,
        };
        var addonTtl = {
            callTtl: 0,
            caseTtl: 0,
        };

        if (totalInfo.transmissions != null) {
            for (x = 0; x < totalInfo.transmissions.length; x++) {
                transTtl.numTrans += totalInfo.transmissions[x].numTrans;
                transTtl.numLate += totalInfo.transmissions[x].numLate;
            };
            transTtl.avgPerc = 1 - (transTtl.numLate / transTtl.numTrans);
            totalInfo.transmissions.ttlTransmissions = transTtl;
        };

        if (totalInfo.deliveries != null) {
            for (y = 0; y < totalInfo.deliveries.length; y++) {
                delivTtl.numDeliv += totalInfo.deliveries[y].numDeliveries;
                delivTtl.caseTtl += totalInfo.deliveries[y].ttlCases;
            };
            delivTtl.avgCase = delivTtl.caseTtl / delivTtl.numDeliv;
            totalInfo.deliveries.ttlDeliveries = delivTtl;
        };

        if (totalInfo.custAddOns != null) {
            for (z = 0; z < totalInfo.custAddOns.length; z++) {
                addonTtl.callTtl += totalInfo.custAddOns[z].numCalls;
                addonTtl.caseTtl += totalInfo.custAddOns[z].ttlCases;
            }
            totalInfo.custAddOns.ttlAddOns = addonTtl;
        };
    };
});

CSRTApp.controller('csrTotalCtrl', function ($scope, $http, $uibModalInstance, $uibModal, $loading, totalInfo) {
    $scope.storeInfos = [totalInfo];
    $scope.storeInfos.fromDate = totalInfo.fromDate;
    $scope.storeInfos.toDate = totalInfo.toDate;

    $scope.close = function () {
        $uibModalInstance.dismiss();
    };

    $scope.requestFile = function () {
        var e = document.getElementById('criteriaSelector');

        $http.post('CustomerServiceReportTotals.aspx/EmailReport', angular.toJson({ fromDate: totalInfo.fromDate, toDate: totalInfo.toDate, desc: totalInfo.desc })).
        then(function (response) {
            alert(response.data.d);
        }, function (response) {
            alert(response.data.d);
        });
    };
});