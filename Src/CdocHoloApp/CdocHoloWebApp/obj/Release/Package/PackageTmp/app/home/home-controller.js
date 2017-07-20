(function () {
    'use strict';

    angular.module('gtbbDashboard')
        .controller('homeController', homeController);

    homeController.$inject = ['$rootScope', '$scope'];

    function homeController($rootScope, $scope) {
        var vm = this;
        $rootScope.title = '';
        vm.error = null;
        vm.$rootScope = $rootScope;
        vm.resourceName = "home"; //this resolves to the resource name on the server



        vm.init();
    }

    var session;

    homeController.prototype = Object.create({
        init: function () {
            var vm = this;
            session = vm.gtbbService.getSession();
        },
        goToDashBoard: function () {
            var self = this;
            self.stateTransitionService.transition("dashboard"); 
        }
    });
})();