Feature: AddTodoItem

User should be able to add todo item

@User
Scenario: As a user I want be able to add todo item
	Given User with todo items
	| Text        |
	| Test item 1 |
	| Test item 2 |
	| Test item 3 |
	When Add todo item with following text: "Test todo item"
	Then User must have 4 todo items
