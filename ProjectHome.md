# Overview #
CCMManager is designed to work alongside SCCM 2007 to help with client problems.

The program will feature the ability to perform actions against a list of clients, and provide feedback on progress.

CCMManager is also developed as a learning experience.

# Features #
CCMManager maintains various lists that are defined by the user.  These lists contain the computers that wish to be managed.  The lists can be populated from Active Directory based on search criteria, or by entry of specific names.

SCCM Related actions that can be performed are include:
  * Policy Download and Apply
  * Policy Reset
  * Initiate DCM Scan
  * Restart the CCMAgent Host

General Actions include:
  * Log current user off
  * Restart
  * Shutdown
  * Wake On LAN (if MAC is stored)
  * Force Group Policy Update (gpupdate /force)

Other features of the app include:
  * View details of a specific computer
  * Get Status of machines before starting tasks to see if they are online or not.
  * Get remote user details.