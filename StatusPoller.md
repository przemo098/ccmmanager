# Introduction #

The StatusPoller attempts to connect to each client in the action list and report its status.


# Details #

A first attempt will be made to ping the client, probably using a WMI method, this can result in 'pingable' or 'offline' messages.

Second, an attempt is made to connect to the ROOT\CIMV2 namespace of the remote computer.  This can result in either an error, or Access Denied message.

Thirdly, a check is performed to confirm that the provided hostname for the machine matches the machine name found in ROOT\CIMV2 "Win32\_ComputerSystem" class object  "DNSHostName".  This can result in either 'Online' or 'DNSError'.

Fourthly, at the same time as above, if not receiving a DNSError, the current logged on user is retreived.

## Implementation ##

The gist of the code is as follows:

```
using System.Threading.Tasks;

List<Task> tasks = new List<Task>();
foreach (object o in SomeList)
{
  var t = Task.Factory.StartNew(() =>
    {
      var result = SomeLongRunningProcess(o);
      this.Dispatcher.BeginInvoke(new Action(() =>
        (Computer)o.Status = States.Online), null);
    });
}
tasks.Add(t);

//If we want to do something when all tasks are done...
Task.Factory.ContinueWhenAll(tasks.ToArray(), result =>
  {
    //Do Something
    this.Dispatcher.BeginInvoke(new Action(() => updatesomething), null);
  }
```

### Updating UI from Long Running Process ###
To update an object on the GUI from a long running process we can simply invoke a new action to take place:
```
this.Dispatcher.BeginInvoke(new Action(() => textBox1.Text += " Updated"));

//Or

App.Current.Dispatcher.BeginInvoke(new Action(() => textBox1.Text += " Updated"));
```