(function () {
    'use strict';

    angular.module('gtbbDashboard')
        .factory('gtbbService', ['$http', '$state', gtbbService]);

    function gtbbService($http, $scope) {

        var session = undefined;

        return {
            login: login,
            getSession: getSession
        };

        function initSession() {
            session = {
                userName: '',
                password: '',
                sessionId: ''
            }
        }

        function getSession() {
            if (!session) {
                initSession();
            }
            return session;
        }

        function login(controllerSession) {
            return $http({
                    url: 'api/Login',
                    method: 'POST',
                    data: controllerSession
                }
            );
        }

        //function ErrorResponse(stateName, NoContentFoundCallback) {
        //    return function (response) {
        //        console.log('401 error response code : ' + response.status);
        //        if (response.status === 401) {
        //            //var toState = $state.get(stateName);
        //            //toState.hasTransitionStateParams = !!toState.saveTransitionStateParams;
        //            //$localStorage.transitionToState = toState;
        //            //adalAuthenticationService.login();
        //        } else if (response.status === 404) { // no content found
        //            // NoContentFoundCallback();
        //        } else {
        //            console.log(response);
        //        }
        //    }
        //}

        //function addAsset(item) {
        //    var asset = {
        //        "dnsName": item.dnsName,
        //        "description": item.description,
        //        "assetType": item.assetType,
        //        "tooling": item.tooling
        //}
        //    ruleSet.assetList.push(asset);
        //}
    }
})();