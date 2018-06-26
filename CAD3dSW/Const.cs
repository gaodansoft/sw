using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CAD3dSW
{
    public class Const
    {
        public const int CAD_MSG_TASK_INIT = 1100;            //任务准备
        public const int CAD_MSG_MODIFY_INIT = 1200;          //零件变化准备
        public const int CAD_MSG_MODIFY = 1300;               //零件变化

        public const int CAD_MSG_OPEN_MODEL = 1;
        public const int CAD_MSG_GET_CFGS = 5;
        public const int CAD_MSG_GET_ITEMS = 6;

        public const string DEFAULTCONFIG = "默认";

        public const int MSG_TYPE_N = 0;// 普通
        public const int MSG_TYPE_WARN = 1;	// 警告
        public const int MSG_TYPE_ERROR = 2;// 错误

    }
}
