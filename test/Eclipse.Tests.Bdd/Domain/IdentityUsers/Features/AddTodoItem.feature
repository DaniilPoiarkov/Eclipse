Feature: AddTodoItem

User should be able to add todo item

@IdentityUser
Scenario: As a user I want be able to add todo item
	Given User with 3 todo items
	When Add todo item with following text: "Test todo item"
	Then User must have 4 todo items
