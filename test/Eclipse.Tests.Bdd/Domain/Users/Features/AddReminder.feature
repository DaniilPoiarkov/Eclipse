Feature: AddReminder

User should be able to add valid reminder

@User
Scenario: As a user I want be able to add reminders
	Given Existing user with 0 reminders
	When Add reminder with following text: "Test reminder"
	And Following time: 17:30
	Then User must have 1 reminders
