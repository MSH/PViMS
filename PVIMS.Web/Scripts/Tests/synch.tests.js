define(
    'synch-tests-function',
    ['jquery','dataservice', 'config', 'Dexie'],
    function ($, dataservice, config, Dexie) {

        //QUnit.config.autostart = false;

        QUnit.test("hello test", function (assert) {
            assert.ok(1 == "1", "Passed!");
        });

        var doNothing = function () { };
       
        var
            fakeTitle = 'fake title';

        var findService = function () {
            return window.testFn($, dataservice, config, Dexie);
        };

        module('synch tests');

        QUnit.test('control tests', function () {
            QUnit.assert.ok(true, 'always passes');
        });

      
        asyncTest('Synch data local storage',
           function () {
               //ARRANGE
               var synch = findService();

               $.mockjax({
                   url: '/api/patientapi',
                   responseTime: 750,
                   responseText:{
                       "patients": [
                           { PatientUniqueIdentifier: uuid.v4(), PatientFirstName: "Siphokuhle", PatientLastName: "Noms" + uuid.v4(), PatientDateOfBirth: "1993-10-02" },
                           { PatientUniqueIdentifier: uuid.v4(), PatientFirstName: "Sindisiwe", PatientLastName: "Saka" + uuid.v4(), PatientDateOfBirth: "1987-09-13" },
                           { PatientUniqueIdentifier: uuid.v4(), PatientFirstName: "Nozipho", PatientLastName: "VAN" + uuid.v4(), PatientDateOfBirth: "1988-03-22" }

                       ]
                   }
               });

               synch.synchPatients()
               .then(function () {

                   ok(true, 'synch patients done');
                   start();


               })
                   .catch(function () {
                       ok(false, 'Synch patients failed');
                       start();
                   });
           }
       );

    });