TABLET
The user attempts the training on a tablet.  When logging in the user is provided with a unique training ID number. This can be used to log back in to the application at a later date to continue training.

At the end of the training session the trainer can log in to the tablets with the admin account.  A summary of the training session that day will be provided to show the individual results for each user in summary form.  Showing a normalised score for donning and doffing. A summary statement will also be provided highlighting which is the worst area from all trainees so that the trainer can provide additional wrap up material.

When the trainer returns to the office they will be able to send the data back to the server by clicking the admin - send button. This will send all XML files to the server.  As an enhancement we can look at archiving old files rather than deleting for perhaps 10 days.

SERVER
As noted above the server receives updates directly from the tablet.

The server supports 3 dashboards for different stakeholders

TRAINER
The trainer dashboard shows data for today's training session and a weekly comparison against previous weeks scores.

Specific data presented include:
Trainees today 
Worst question today
Worst section today
Stacked bar chart of donning scores (main piece of dashboard)
Stacked bar chart of doffing scores (main piece of dashboard)
Donning order line chart
Doffing order line chart
Weekly combined time taken to complete the test

Note a section refers to each piece of equipment for donning/doffing.  So we can tell that doffing gown is a problem area.

If possible we could provide a map of west Africa with coloured highlights on the geo location to depict competency level

PROGRAMME MANAGER 
The programme manager dashboard shows information at a wider programme level.  At a later date we can expand it to show multiple country locations.

Specific data presented include:
Trainees since start of programme
Trainees this week
Split male/female
Average age
Donning order line chart
Doffing order line chart
Worst section this week
Weekly Bar chart showing combined scores
Stacked bar chart of donning scores (main piece of dashboard)
Stacked bar chart of doffing scores (main piece of dashboard)
Weekly combined time taken to complete the test

APPLICATION DEVELOPER
This dashboard shows the maximum amount of information to allow the developer to fine tune the questions and see improvements as changes are made to the application.

Show line chart for answers per section 
Show timings for each section
Show combined number of times question is answered incorrectly
Show section that takes the longest

RETENTION TEXT
The system sends a reminder to the users mobile number after a configurable number of weeks since their last test

REMEDIAL TEXT
If the user has only attained a certain score on a specific section the system will send a series of questions to the users mobile in an attempt to remind them of the section guessed incorrectly.

These questions will be based upon which questions the user got wrong during the test.  If for example the user incorrectly answered the face shield doffing questions then we will send them appropriate questions.

The application will then receive a response from the end user via SMS this should be logged against their unique id to show progress.

If the user still getting the answers wrong we need to notify the trainer via the dashboard that they have trainees who need remedial attention.

MOBILE
The application needs to send basic text messages to the users phone unless they have a phone with picture text enabled.  If the later then the test pictures will be sent