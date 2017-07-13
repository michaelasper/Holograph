//
//  Copyright (C) Microsoft. All rights reserved.    
// 

(function () {
    'use strict';

    angular.module('gtbbDashboard', ['ui.router', 'ui.bootstrap', 'ngAnimate'])
    .constant('constants', {
        apiBaseUrl: 'api/'
    })
        .config(config)
        .run(run);

    config.$inject = ['$stateProvider'];

    function config($stateProvider) {

        $stateProvider
            .state('index',
            {
                views: {
                    "": {
                        templateUrl: "../app/home/home.html",
                        controller: 'homeController',
                        controllerAs: 'vm'
                    }
                },
                url: "/"
            })
            .state('home',
            {
                url: "/home",
                templateUrl: "../app/home/home.html",
                controller: 'homeController',
                controllerAs: 'vm'
            })
            .state('dashboard',
            {
                url: "/dashboard",
                templateUrl: "../app/dashboard/dashboard.html",
                controller: 'dashboardController',
                controllerAs: 'vm'
            })
            .state('login',
            {
                url: "/rules",
                templateUrl: "../app/login/login.html",
                controller: 'loginController',
                controllerAs: 'vm',
            });
    }

    run.$inject = ['$rootScope', '$state'];

    function run($rootScope, $state) {
        $rootScope.title = 'Green Team Baseball';
        $rootScope.$state = $state;
    }
    
})();