var app = angular.module('startApp', ['smart-table']);

app.controller('employeeCtrl', ['$scope', '$http',
    function ($scope, $http) {
        $scope.displayedCollection = [].concat($scope.employeesCollection);

        $scope.searchEmp = function () {

            var first = document.getElementById("FirstName").value;
            var last = document.getElementById("LastName").value;
            var ex = document.getElementById("Extension").value;
            var store = document.getElementById("store");
            var st = store.options[store.selectedIndex].value;
            var number = document.getElementById("storeNo");
            var num = number.options[number.selectedIndex].value;

            $http({
                method: 'GET',
                url: 'getEmployees.aspx',
                params: { f: first, q: last, e: ex, s: st, n: num }
            }).then(function (response) {
                console.log(response.data);
                $scope.employeesCollection = response.data;
            });
        };
    }]);