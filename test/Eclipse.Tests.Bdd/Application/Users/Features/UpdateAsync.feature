Feature: UpdateAsync

User should be able to update personal info and persist in database

@User
Scenario: As a user I want to be able to update my own info
	Given An existing user with followind data:
		| Id                                   | Name | Surname | Username | ChatId |
		| 77134e03-551f-4cc2-aefd-7841fe864af4 | Name | Surname | Username | 1      |
	When I update the user with the following details:
		| Name             | Surname      | Username   | Culture  | NotificationsEnabled |
		| NewName          | NewSurname   | NewUsername| en-US	  | true                 |
	Then The user details should be updated successfully
	And The user's name should be "NewName"
    And The user's surname should be "NewSurname"
    And The user's username should be "NewUsername"
    And The user's culture should be "en-US"
    And The user's notification status should be true
