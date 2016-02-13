# Introduction #
This Page is to keep a track of some items to be completed.



# Details #

### UI ###
  * Computer Details Tab Layout.
  * Make everything uniform.
  * Implement appropriate resource dictionaries for styling.

### StatusPoller ###
  * Change from Text feedback to Image.
  * Incorporate current User details (maybe in a tooltip?).
  * Add methods for retrieving the MAC address of the remote client, and adding it to the repository for future storage.
  * **Change to Task Parallel Library (TPL)**
> StatusPoller and ActionPoller become One and the same.

### ActionPoller ###
  * Form a way of calling/setting multiple actions per client and reporting status in the GUI.
> Has to be done on a background thread so as not to hang the UI.
> Similar to the StatusPoller.