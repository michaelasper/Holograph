(function() {
    'use strict';

    angular.module('gtbbDashboard')
        .controller('loginController', loginController);

    loginController.$inject = [
        '$rootScope', 'constants', '$state', '$stateParams', 'gtbbService'
    ];

    function loginController($rootScope,
        constants,
        $state,
        $stateParams,
        gtbbService) {
        var vm = this;
        vm.error = null;
        vm.resourceName = "engagement";
        vm.$rootScope = $rootScope;
        vm.$state = $state;
        vm.$stateParams = $stateParams;
        vm.gtbbService = gtbbService;

        vm.session = vm.gtbbService.getSession();

        vm.init();
    }

    loginController.prototype = Object.create({
        init: function() {
            var vm = this;
            //var holder = vm.gtbbService.getSession();
            //vm.session = holder;
        },

        login: function() {
            var vm = this;
            var holder = vm.gtbbService.login(vm.session);
            holder.then(function(response) {
                vm.session = response.data;
            });
        }
        
        //loadCase: function() {
        //    var vm = this;
        //    vm.cases = vm.rulesService.getCase();
        //    vm.generateFacts(vm.cases);
        //},
    });
})();