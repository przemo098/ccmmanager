using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace CCMManager.Models.Actions
{
    public class PolicyDownloadAction : RemoteAction
    {
         public PolicyDownloadAction(Dictionary<RemoteActionState, ImageSource> images)
            :base(images)
        {
        }

        public override void Execute(object context = null)
        {

        }
    }
}
