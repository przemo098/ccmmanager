using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace CCMManager.Models.Actions
{
    public class GPUpdateAction : RemoteAction
    {
        public GPUpdateAction(Dictionary<RemoteActionState, ImageSource> images)
            :base(images)
        {
        }

        public override void Execute(object context = null)
        {

        }
    }
}
