Feature: TryParseAsTimeOnly

Check whether extension methods passes happy case

@Extensions
Scenario: As a developer I want to use method to parse string value as TimeOnly
	Given Following string: 17:30
	When Call TryParseAsTimeOnly extension method
	Then Bollean indicates as true and outcome value must contain 17 hours and 30 minutes
