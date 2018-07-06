using Offline.WebUI.ActionResults;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Offline.WebUI.Controllers
{
    public class ResourcesController : Controller
    {
        public ActionResult Manifest()
        {
            var version = GetType().Assembly.GetName().Version.ToString();

            var result = new ManifestResult(version)
            {
                CacheResources = new List<string> 
                {
                    Url.Action("Index", "Offline"),
                    Url.Action("PatientSearch", "Patient"),
                    Url.Action("PatientView","Patient"),
                    Url.Action("EncounterSearch", "Encounter"),
                    Url.Action("AddEncounterOffline","Encounter"), 
                    Url.Action("ViewEncounterOffline", "Encounter"),
                    Url.Action("PatientClinicalEventOffline","PatientClinicalEvent"),
                    Url.Action("PatientConditionView","PatientCondition"),
                    Url.Action("PatientLabTestOffline","PatientLabTest"),
                    Url.Action("PatientMedicationView","PatientMedication"),

                    Scripts.Url("~/Scripts/jquery-2.1.3.min.js").ToString(),
                    Scripts.Url("~/Scripts/jquery-2.1.3.js").ToString(),
                    Scripts.Url("~/Scripts/jquery-2.1.4.min.js").ToString(),
                    Scripts.Url("~/Scripts/jquery-2.1.4.js").ToString(),
                    Scripts.Url("~/Scripts/jquery.validate.unobtrusive.js").ToString(),
                    Scripts.Url("~/js/libs/jquery-ui-1.10.3.min.js").ToString(),
                    Scripts.Url("~/js/app.config.js").ToString(),
                    Scripts.Url("~/js/plugin/jquery-touch/jquery.ui.touch-punch.min.js").ToString(),
                    Scripts.Url("~/js/notification/SmartNotification.min.js").ToString(),
                    Scripts.Url("~/js/smartwidgets/jarvis.widget.min.js").ToString(),
                    Scripts.Url("~/js/plugin/easy-pie-chart/jquery.easy-pie-chart.min.js").ToString(),
                    Scripts.Url("~/js/plugin/sparkline/jquery.sparkline.min.js").ToString(),
                    Scripts.Url("~/Scripts/jquery.validate.min.js").ToString(),
                    Scripts.Url("~/js/plugin/masked-input/jquery.maskedinput.min.js").ToString(),
                    Scripts.Url("~/js/plugin/select2/select2.min.js").ToString(),
                    Scripts.Url("~/js/plugin/bootstrap-slider/bootstrap-slider.min.js").ToString(),
                    Scripts.Url("~/js/plugin/msie-fix/jquery.mb.browser.min.js").ToString(),
                    Scripts.Url("~/js/plugin/fastclick/fastclick.min.js").ToString(),
                    Scripts.Url("~/js/plugin/bootstraptree/bootstrap-tree.min.js").ToString(),
                    Scripts.Url("~/js/app.min.js").ToString(),
                    Scripts.Url("~/js/speech/voicecommand.min.js").ToString(),
                    Scripts.Url("~/Scripts/jquery.dataTables.js").ToString(),
                    Scripts.Url("~/js/plugin/datatables/dataTables.colVis.min.js").ToString(),
                    Scripts.Url("~/js/plugin/datatables/dataTables.tableTools.min.js").ToString(),
                    Scripts.Url("~/js/plugin/datatables/dataTables.bootstrap.min.js").ToString(),
                    Scripts.Url("~/js/plugin/datatable-responsive/datatables.responsive.min.js").ToString(),
                    Scripts.Url("~/js/plugin/x-editable/x-editable.min.js").ToString(),
                    Scripts.Url("~/Scripts/modernizr-2.6.2.js").ToString(),
                    Scripts.Url("~/Scripts/modernizr-2.8.3.js").ToString(),
                    Scripts.Url("~/Scripts/bootstrap.min.js").ToString(),
                    Scripts.Url("~/Scripts/ckeditor/ckeditor.js").ToString(),
                    Scripts.Url("~/Scripts/ckeditor/plugins/styles/styles/default.js?t=C9A85WF").ToString(),
                    Scripts.Url("~/Scripts/ckeditor/config.js?t=C9A85WF").ToString(),
                    Scripts.Url("~/Scripts/ckeditor/lang/en.js?t=C9A85WF").ToString(),
                    Scripts.Url("~/Scripts/ckeditor/adapters/jquery.js").ToString(),
                    Scripts.Url("~/Scripts/knockout-3.3.0.js").ToString(),    
                    Scripts.Url("~/Scripts/knockout/binder.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/bootstrapper.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/config.js").ToString(),                    
                    Scripts.Url("~/Scripts/knockout/db.js").ToString(),
                    Scripts.Url("~/Scripts/main.js").ToString(),                    
                    Scripts.Url("~/Scripts/knockout/synch.js").ToString(),
                    Scripts.Url("~/Scripts/offline.js").ToString(),
                    Scripts.Url("~/Scripts/synchronisation.js").ToString(),                    
                    Scripts.Url("~/Scripts/knockout.dirtyFlag.js").ToString(),
                    Scripts.Url("~/Scripts/knockout.validation.js").ToString(),
                    Scripts.Url("~/Scripts/moment.js").ToString(),
                    Scripts.Url("~/Scripts/uuid.js").ToString(),
                    Scripts.Url("~/Scripts/underscore-min.js").ToString(),
                    Scripts.Url("~/Scripts/require.js").ToString(),  
                    Scripts.Url("~/Scripts/knockout/dataservice.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.patient.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.common.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.customAttribute.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.encounter.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.patientClinicalEvent.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.patientCondition.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.patientLabTest.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/dataservice.patientMedication.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.mapper.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.encounter.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.patient.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.patientClinicalEvent.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.patientCondition.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.patientLabTest.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/model.patientMedication.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/utils.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.addEncounter.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.encounter.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.patient.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.patientClinicalEvent.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.patientCondition.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.patientLabTest.js").ToString(),
                    Scripts.Url("~/Scripts/knockout/vm.patientMedication.js").ToString(),
                    //Scripts.Url("~/bundles/jsapplibs").ToString(),
                    Scripts.Url("~/Scripts/site.js").ToString(),
                    Scripts.Url("~/Scripts/Dexie.js").ToString(),
                    //Scripts.Url("~/Scripts/jquery.validate.js").ToString(),
                    Scripts.Url("~/Scripts/notify.min.js").ToString(),

                    Styles.Url("~/css/bootstrap.min.css").ToString(),
                    Styles.Url("~/css/font-awesome.min.css").ToString(),
                    Styles.Url("~/css/smartadmin-production.min.css").ToString(),
                    Styles.Url("~/css/smartadmin-skins.min.css").ToString(),
                    Styles.Url("~/css/notifyjs.css").ToString(),
                    Styles.Url("~/Scripts/ckeditor/contents.css").ToString(),
                    Styles.Url("~/Scripts/ckeditor/skins/kama/editor.css?t=C9A85WF").ToString(),

                    // Fonts
                    Styles.Url("~/img/favicon/favicon.ico").ToString(),
                    Styles.Url("~/fonts/fontawesome-webfont.eot").ToString(),
                    Styles.Url("~/fonts/fontawesome-webfont.svg").ToString(),
                    Styles.Url("~/fonts/fontawesome-webfont.ttf").ToString(),
                    Styles.Url("~/fonts/fontawesome-webfont.woff").ToString(),
                    Styles.Url("~/fonts/fontawesome-webfont.woff?v=4.1.0").ToString(),
                    Styles.Url("~/fonts/FontAwesome.otf").ToString(),
                    Styles.Url("~/fonts/free3of9.ttf").ToString(),
                    Styles.Url("~/fonts/glyphicons-halflings-regular.eot").ToString(),
                    Styles.Url("~/fonts/glyphicons-halflings-regular.svg").ToString(),
                    Styles.Url("~/fonts/glyphicons-halflings-regular.ttf").ToString(),
                    Styles.Url("~/fonts/glyphicons-halflings-regular.woff").ToString(),

                    // images
                    Styles.Url("~/img/logo-o.png").ToString(),
                    Styles.Url("~/img/avatars/male.png").ToString(),
                    Styles.Url("~/img/blank.gif").ToString(),
                    Styles.Url("~/img/mybg.png").ToString(),
                    Styles.Url("~/img/SIAPS_USAID_Small.jpg").ToString(),
                    Styles.Url("~/img/flags/flags.png").ToString(),
                    Styles.Url("~/Scripts/ckeditor/skins/kama/images/sprites.png").ToString(),
                    Styles.Url("~/Scripts/ckeditor/skins/kama/icons.png").ToString()
                },
                FallbackResources = new Dictionary<string, string> 
                {
                    //{ Styles.Url("~/fonts/fontawesome-webfont.eot").ToString(), Styles.Url("~/fonts/fontawesome-webfont.eot").ToString() },
                    //{ Styles.Url("~/fonts/fontawesome-webfont.svg").ToString(), Styles.Url("~/fonts/fontawesome-webfont.svg").ToString() },
                    //{ Styles.Url("~/fonts/fontawesome-webfont.ttf").ToString(), Styles.Url("~/fonts/fontawesome-webfont.ttf").ToString() },
                    //{ Styles.Url("~/fonts/fontawesome-webfont.woff").ToString(), Styles.Url("~/fonts/fontawesome-webfont.woff").ToString() },
                    //{ Styles.Url("~/fonts/fontawesome-webfont.woff?v=4.1.0").ToString(), Styles.Url("~/fonts/fontawesome-webfont.woff?v=4.1.0").ToString() },
                    { Styles.Url("~/fonts/FontAwesome.otf").ToString(), Styles.Url("~/fonts/FontAwesome.otf").ToString() },
                    { Styles.Url("~/fonts/glyphicons-halflings-regular.eot").ToString(), Styles.Url("~/fonts/glyphicons-halflings-regular.eot").ToString() },
                    { Styles.Url("~/fonts/glyphicons-halflings-regular.svg").ToString(), Styles.Url("~/fonts/glyphicons-halflings-regular.svg").ToString() },
                    { Styles.Url("~/fonts/glyphicons-halflings-regular.ttf").ToString(), Styles.Url("~/fonts/glyphicons-halflings-regular.ttf").ToString() },
                    { Styles.Url("~/fonts/glyphicons-halflings-regular.woff").ToString(), Styles.Url("~/fonts/glyphicons-halflings-regular.woff").ToString() }
                },
                NetworkResources = new[] { "*" , "/api", "/favicon.ico" }
            };

            return result;
        }
    }
}