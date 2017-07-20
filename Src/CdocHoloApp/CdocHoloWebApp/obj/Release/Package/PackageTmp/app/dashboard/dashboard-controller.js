(function() {
    'use strict';

    angular.module('gtbbDashboard')
        .controller('dashboardController', dashboardController);

    dashboardController.$inject = [
        '$rootScope', 'constants', '$state', '$stateParams', 'gtbbService'
    ];

    function dashboardController($rootScope,
        constants,
        $state,
        $stateParams,
        gtbbService) {
        var vm = this;
        vm.$rootScope = $rootScope;
        vm.$state = $state;
        vm.$stateParams = $stateParams;
        vm.gtbbService = gtbbService;

        //vm.initNewdashboard();
        vm.init();
    }

    dashboardController.prototype = Object.create({
        init: function () {
            var vm = this;
            
            //if (!vm.newdashboard.radioMode) {
            //    vm.newdashboard.radioMode = 'form';
            //}
            //vm.$state.go('.' + vm.newdashboard.radioMode);

            //vm.buttons = [{ "label": "IP/Content Abuse", "type": "cars" }, { "label": "Pentest Request", "type": "pentest" }];

            //if (vm.$rootScope.userInfo.isAuthenticated) {
            //    vm.buttons.push({ "label": "Prior dashboards", "type": "dashboards" });
            //}
        },

        initNewdashboard: function() {
            var vm = this;
            //vm.newDashboard = vm.gtbbService.createDashboardDataModel();
        }
    });
})();