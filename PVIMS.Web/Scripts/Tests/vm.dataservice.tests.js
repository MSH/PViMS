define(
    'vm-dataservice-tests-function',
    ['jquery','dataservice.patient','db'],
    function ($, dataservicePatient, db) {

        //QUnit.config.autostart = false;

        QUnit.test("hello test", function (assert) {
            assert.ok(1 == "1", "Passed!");
        });

        var doNothing = function () { };
       
        var
            fakeTitle = 'fake title';

        var findService = function () {
            return window.testFn(dataservicePatient);
        };

        module('data service tests');

        QUnit.test('control tests', function () {
            QUnit.assert.ok(true, 'always passes');
        });

        asyncTest('Load patient by uniqueidentifier and firstname is checked and correct',
            function () {
                //ARRANGE
                var service = findService();

                var id = '9D1A6775-88F7-428E-A1BE-DBB437960D08'
                service.dataServicePatient.localService.getByUniqueIdentifier(id)
                    .done(function (data) {
                        ok(data && data.PatientFirstName === 'Sindisiwe', 'Got patients');
                        start();
                    })
                    .fail(function (data) {
                        ok(false, 'failed!');
                        start();
                    });                
              
            }
        );

        asyncTest('Load patient search by unique identifier criteria',
            function () {
                //ARRANGE
                var service = findService();

                var id = '9D1A6775-88F7-428E-A1BE-DBB437960D08';
                criteria = [{ PatientUniqueIdentifier: id }];

                service.dataServicePatient.localService.searchByIds(criteria, true)
                    .done(function (data) {
                        ok(data && data.PatientFirstName === 'Sindisiwe', 'Got patients');
                        start();
                    })
                    .fail(function (data) {
                        ok(false, 'failed!');
                        start();
                    });

            }
        );

        asyncTest('Load patient search by many criteria return One',
            function () {
                //ARRANGE
                var service = findService();
                
                criteria = [{ PatientFirstName: "Sindi" },
                            { PatientLastName: "Sikh" }];

                service.dataServicePatient.localService.searchByIds(criteria)
                    .done(function (data) {
                        ok(data && data.length === 1, 'Got patients');
                        start();
                    })
                    .fail(function (data) {
                        ok(false, 'failed!');
                        start();
                    });

            }
        );

        asyncTest('Load patient search by many criteria return more than one',
            function () {
                //ARRANGE
                var service = findService();

                criteria = [{ PatientFirstName: "si" },
                            { PatientLastName: "si" }];

                service.dataServicePatient.localService.searchByIds(criteria)
                    .done(function (data) {
                        ok(data && data.length >= 0, 'Got ' + data.length + ' patients' );
                        start();
                    })
                    .fail(function (data) {
                        ok(false, 'failed!');
                        start();
                    });

            }
        );

        asyncTest('Load patient by uniqueidentifier and firstname is checked and incorrect',
            function () {
                //ARRANGE
                var service = findService();

                var id = '9D1A6775-88F7-428E-A1BE-DBB437960D08'
                service.dataServicePatient.localService.getByUniqueIdentifier(id)
                    .done(function (data) {
                        ok(data && data.PatientFirstName != 'fake', 'Got patients invalid FirstName');
                        start();
                    })
                    .fail(function (data) {
                        ok(false, 'failed!');
                        start();
                    });

            }
        );

        asyncTest('Load all patients and custom attributes, verify for object pos=1 has at least one',
            function () {
                //ARRANGE
                var service = findService();

                service.dataServicePatient.localService.getData()
                    .done(function (data) {
                        ok(data && data[1].customAttributes.length > 0, 'Get data returns results');
                        start();
                    })
                    .fail(function (data) {
                        ok(false, 'failed!');
                        start();
                    });

            }
        );

        asyncTest('Test synch data addBatch function',
           function () {
               //ARRANGE
               var service = findService();

               var init = function () {
                   $.mockJSON.random = false;
                   $.mockJSON.log = false;
                   $.mockJSON.data.MALEFIRSTNAME = ['John', 'Dan', 'Scott', 'Hans', 'Ward', 'Jim', 'Ryan', 'Steve', 'Ella', 'Landon', 'Haley', 'Madelyn'];
                   $.mockJSON.data.LASTNAME = ['Guthrie', 'Fjällemark', 'Bell', 'Cowart', 'Niemeyer', 'Sanderson','Morgan','Costa','Wass'];                   
                   $.mockJSON.data.DATE_FULL = [new Date()];                  
                   $.mockJSON.data.GENDER = ['F', 'M'];
                   $.mockJSON.data.UNIQUEKEY = [uuid.v4(), uuid.v4(), uuid.v4(), uuid.v4(), uuid.v4(), uuid.v4(), uuid.v4(), uuid.v4(), uuid.v4(), uuid.v4()];
               };

               init();

               $.mockjax({
                   url: '/api/patientapi',
                   responseTime: 750,
                   responseText: $.mockJSON.generateFromTemplate({
                       "patients|2-3": [{                          
                           "PatientUniqueIdentifier": "@UNIQUEKEY",
                           "PatientFirstName": "@MALEFIRSTNAME",
                           "PatientLastName|+1": "@LASTNAME",
                           "PatientDateOfBirth": '@DATE_FULL'
                       }]
                   })
               });

               $.ajax({
                   url: "/api/patientapi"
               }).done(function (data) {
                       
                   // TODO: Improve this unit test to run multiple times and produce correct outcome
                            // Now can be execute one time

                    service.dataServicePatient.localService.addBatch(data.patients)
                           .done(function () {
                               db.patients.count(function (count) {
                                   ok(count -3 === data.patients.length, "Successfully batch insertion"); // 3 items previously added on database creation
                                   start();
                               });
                               //db.delete(); // delete database; for test accuracy delete after test. Danger for Production
                           })
                           .fail(function () {
                               ok(false, 'failed batch insertion, Should run on new database');
                               start();
                           });

                   })
                   .fail(function (data) {
                       ok(false, 'failed!');
                       start();
                   });

           }
       );

    });