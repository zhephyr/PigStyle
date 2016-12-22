angular.module('plumApp', ['ngAnimate', 'ui.bootstrap', 'darthwade.loading']);

angular.module('plumApp').controller('pluModalCtrl', function ($scope, $uibModal, $http, $loading) {

    $scope.getDepts = function () {
        $http.post('plu.aspx/GetDepts', { responseType: 'json' }).
        then(function (response) {
            $scope.depts = JSON.parse(response.data.d)
        });
    };

    $scope.getDepts();

    $scope.pluTxt = "";

    $scope.checkSubmit = function (keyEvent) {
        if (keyEvent.which === 13 && $scope.pluTxt != "") {
            keyEvent.preventDefault();
            $scope.checkDuplicates($scope.pluTxt);
        }
        else if (keyEvent.which === 13 && $scope.descTxt == "") {
            alert("That is not a valid submission.")
        }
    };

    $scope.checkDuplicates = function (pluno) {
        $loading.start('plumLoading');
        $http.post('plu.aspx/CheckDuplicates', angular.toJson({ pluno: pluno })).
        then(function (response) {
            var data = JSON.parse(response.data.d);

            if (data === null)
                alert("PLU number " + pluno + " does not exist!");
            else {
                if (data.length > 1)
                    $scope.loadChoices(data);
                else
                    $scope.loadData(data);
            };
            $loading.finish('plumLoading');
        });
    };

    $scope.descTxt = "";
    $scope.deptChoice = "";
    $scope.pluSort = false;

    $scope.checkDesc = function (keyEvent) {
        if (keyEvent.which === 13 && $scope.descTxt != "") {
            keyEvent.preventDefault();
            $scope.loadPLUs();
        }
        else if (keyEvent.which === 13 && $scope.descTxt == "") {
            alert("That is not a valid submission.")
        }
    };

    $scope.pluVisible = false;

    $scope.checkLoaded = function () {
        if ($scope.pluVisible)
            $scope.loadPLUs();
    };

    $scope.loadPLUs = function () {
        $loading.start('plumLoading');
        $http.post('plu.aspx/LoadPLUs', angular.toJson({ desc: $scope.descTxt, dept: $scope.deptChoice, checked: $scope.pluSort })).
        then(function (response) {
            $scope.pluList = JSON.parse(response.data.d);
            $scope.pluVisible = true;
            $loading.finish('plumLoading');
        });
    };

    $scope.pluChoice = "";

    $scope.submitPLU = function () {
        var pluInfo = {
            pluno: $scope.pluChoice,
            deptname: $scope.deptChoice,
        };

        $loading.start('plumLoading');
        $http.post('plu.aspx/LoadData', angular.toJson(pluInfo)).
        then(function (response) {
            var data = JSON.parse(response.data.d);

            $scope.loadData(data);
            $loading.finish('plumLoading');
        });
    };

    $scope.loadData = function (data) {
        $scope.productData = data;

        var pluInstance = $uibModal.open({
            animation: true,
            templateUrl: 'PLU-Data.html',
            controller: 'pluInstanceCtrl',
            size: 'lg',
            resolve: {
                productData: function () {
                    return $scope.productData;
                }
            }
        });
    };

    $scope.loadChoices = function (data) {
        $scope.plus = data;

        var pluInstance = $uibModal.open({
            animation: true,
            templateUrl: 'pluChoices.html',
            controller: 'pluInstanceCtrl',
            size: 'lg',
            resolve: {
                productData: function () {
                    return $scope.plus;
                }
            },
        });

        pluInstance.result.then(function (data) {
            $loading.start('plumLoading');
            $http.post('plu.aspx/LoadData', angular.toJson(data)).
            then(function (response) {
                $scope.data = JSON.parse(response.data.d);

                $scope.loadData($scope.data);
                $loading.finish('plumLoading');
            });
        });
    };

    $scope.reset = function () {
        $scope.pluTxt = "";
        $scope.descTxt = "";
        $scope.deptChoice = "";
        $scope.pluSort = false;
        $scope.pluVisible = false;
        $scope.pluChoice = "";
    };
});

angular.module('plumApp').controller('pluInstanceCtrl', function ($scope, $uibModalInstance, productData) {
    $scope.productData = productData;

    if ($scope.productData != null) {
        $scope.pluValid = true;
    };

    if (String($scope.productData.UPC).length <= 5)
        $scope.productData.UPC = "2 " + productData.UPC + " 00000"

    if ($scope.productData.ServSize != null) {
        $scope.areFacts = true;
    };

    $scope.select = function (selectedPLU, selectedDept) {
        $scope.data = {
            pluno: selectedPLU,
            deptname: selectedDept
        };

        $uibModalInstance.close($scope.data);
    };

    $scope.dismiss = function () {
        $uibModalInstance.dismiss();
    };

    $scope.print = function () {
        $scope.printElement(document.getElementById('modalHeader'));
        $scope.printElement(document.getElementById('modalBody'));
        window.print();
        document.getElementById('printSection').innerHTML = "";
    };

    $scope.printElement = function (elem) {
        var domClone = elem.cloneNode(true);

        var $printSection = document.getElementById('printSection');

        if (!$printSection) {
            var $printSection = document.createElement('div');
            $printSection.id = 'printSection';
            document.body.appendChild($printSection);
        }

        $printSection.appendChild(domClone);
    };
});