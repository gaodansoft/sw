using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Diagnostics;




namespace CAD3dSW.Controler
{
    class ReadModelInfo
    {

        #region 提取尺寸列表
        public string getdimlist(ISldWorks iSwApp, ModelDoc2 model)
        {
            List<string> dimlist = new List<string>();
            ModelDoc2 swModel;
            if (model != null)
            {
                swModel = model;
            }
            else
            {
                swModel = iSwApp.ActiveDoc as ModelDoc2;
            }

            Feature swFeat = swModel.FirstFeature() as Feature;

            do
            {
                if (swFeat != null)
                {
                    Feature swSubFeat = swFeat.GetFirstSubFeature() as Feature;
                    if (swSubFeat != null)
                    {
                        addDimList(swSubFeat, dimlist);
                    }

                    addDimList(swFeat, dimlist);

                }
                swFeat = swFeat.GetNextFeature() as Feature;

            } while (swFeat != null);

            //dimlist.Distinct<>;
            var DL = (from p in dimlist
                      select p)
                .Distinct();

            string rl = string.Empty;

            foreach (var item in DL)
            {
                rl = rl + "'" + item + "',";
            }
            return rl;
        }

        private void addDimList(Feature ft, List<string> dimlist)
        {
            do
            {
                DisplayDimension swDispDim = ft.GetFirstDisplayDimension() as DisplayDimension;
                if (swDispDim != null)
                {
                    do
                    {
                        //Annotation swAnn = swDispDim.GetAnnotation() as Annotation;
                        Dimension swDim = swDispDim.GetDimension() as Dimension;
                        dimlist.Add(swDim.FullName);
                        swDispDim = ft.GetNextDisplayDimension(swDispDim) as DisplayDimension;

                    } while (swDispDim != null);
                }
                ft = ft.GetNextSubFeature() as Feature;

            } while (ft != null);
        }
        #endregion


        #region 提取特征及零部件列表

        public string getFeatureList(ISldWorks iSwApp, ModelDoc2 model)
        {
            string FeatureRL = string.Empty;

            List<string> FeatureList = new List<string>(); ;

            ModelDoc2 swModel;
            if (model != null)
            {
                swModel = model;
            }
            else
            {
                swModel = iSwApp.ActiveDoc as ModelDoc2;
            }

            Feature swFeat = swModel.FirstFeature() as Feature;

            do
            {
                if (swFeat != null)
                {
                    FeatureList.Add(swFeat.Name);
                }
                swFeat = swFeat.GetNextFeature() as Feature;

            } while (swFeat != null);

            //dimlist.Distinct<>;
            var DL = (from p in FeatureList
                      select p)
                .Distinct();


            foreach (var item in DL)
            {
                FeatureRL = FeatureRL + "'" + item + "',";
            }

            return FeatureRL;

        }

        #endregion


        #region 批量修改此配置
       
        private void ChangeDimConfigurations(Feature ft)
        {
            DisplayDimension swDispDim = ft.GetFirstDisplayDimension() as DisplayDimension;
            if (swDispDim != null)
            {
                do
                {
                    //Annotation swAnn = swDispDim.GetAnnotation() as Annotation;
                    Dimension swDim = swDispDim.GetDimension() as Dimension;

                    if (!swDim.ReadOnly)
                    {
                        swDim.SetSystemValue3(swDim.SystemValue, 1, null);
                    }
                    //swDim.SetValue3(swDim.Value , 1, null);
                    //System.Windows.Forms.MessageBox.Show(swDim.Value.ToString());
                    swDispDim = ft.GetNextDisplayDimension(swDispDim) as DisplayDimension;
                } while (swDispDim != null);
            }
        }


        /// <summary>
        /// 遍历
        /// </summary>
        /// <param name="swFeat"></param>
        /// <param name="nLevel"></param>
        
        public void TraverseFeatures(Feature swFeat, long nLevel)
        {

            Feature swSubFeat;

           

            string sPadStr = " ";

            long i = 0;

            for (i = 0; i <= nLevel; i++)
            {

                sPadStr = sPadStr + " ";

            }

            while ((swFeat != null))
            {
                //获得特征
                string aa = (sPadStr + swFeat.Name + " [" + swFeat.GetTypeName2() + "]");

                swSubFeat = (Feature)swFeat.GetFirstSubFeature();

                if ((swSubFeat != null))
                {

                    TraverseFeatures(swSubFeat, nLevel + 1);

                }

                if (nLevel == 1)
                {

                    swFeat = (Feature)swFeat.GetNextFeature();

                }

                else
                {

                    swFeat = (Feature)swFeat.GetNextSubFeature();

                }

                if (swFeat != null)
                {
                    ChangeDimConfigurations(swFeat);
                }

            }

        }



        public void TraverseComponentFeatures(Component2 swComp, long nLevel)
        {

            Feature swFeat;

            swFeat = (Feature)swComp.FirstFeature();

            TraverseFeatures(swFeat, nLevel);

        }



        public void TraverseComponent(Component2 swComp, long nLevel)
        {

            object[] vChildComp;

            Component2 swChildComp;

            string sPadStr = " ";

            long i = 0;

            for (i = 0; i <= nLevel - 1; i++)
            {

                sPadStr = sPadStr + " ";

            }

            vChildComp = (object[])swComp.GetChildren();

            for (i = 0; i < vChildComp.Length; i++)
            {

                swChildComp = (Component2)vChildComp[i];
                //获得零件
                string aa = (sPadStr + "+" + swChildComp.Name2 + " <" + swChildComp.ReferencedConfiguration + ">");

                TraverseComponentFeatures(swChildComp, nLevel);

                TraverseComponent(swChildComp, nLevel + 1);

            }

        }



        public void TraverseModelFeatures(ModelDoc2 swModel, long nLevel)
        {

            Feature swFeat;

            swFeat = (Feature)swModel.FirstFeature();

            TraverseFeatures(swFeat, nLevel);

        }

        public void Main(ISldWorks iSwApp)
        {

            //System.Windows.Forms.MessageBox.Show("Test");
            if (System.Windows.Forms.MessageBox.Show("是否确认将所有尺寸置为此配置？", "提示：此操作不可逆", 
                System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }
                
            ModelDoc2 swModel;

            ConfigurationManager swConfMgr;

            Configuration swConf;

            Component2 swRootComp;



            swModel = (ModelDoc2)iSwApp.ActiveDoc;

            swConfMgr = (ConfigurationManager)swModel.ConfigurationManager;

            swConf = (Configuration)swConfMgr.ActiveConfiguration;

            swRootComp = (Component2)swConf.GetRootComponent3(true);
            

            System.Diagnostics.Stopwatch myStopwatch = new Stopwatch();

            myStopwatch.Start();


            //获得装配体
            string aa = ("File = " + swModel.GetPathName());

            TraverseModelFeatures(swModel, 1);

            if (swModel.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
            {

                TraverseComponent(swRootComp, 1);

            }



            myStopwatch.Stop();

            TimeSpan myTimespan = myStopwatch.Elapsed;

            aa = ("Time = " + myTimespan.TotalSeconds + " sec");

            System.Windows.Forms.MessageBox.Show("修改完成！请保存模型。");

        }
        
        #endregion

    }
}
