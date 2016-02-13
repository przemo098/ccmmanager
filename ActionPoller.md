# Introduction #

The ActionPoller drives the applications remote action requests.

# Details #

The right hand menu provides a list of actions that can be performed on the client(s).
Each of these sends a request to the ActionPoller to be completed on a background thread.
These threads, are managed by the ThreadPool.QueueUserWorkItem() method.