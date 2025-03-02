# Direwolf
Framework for data management of Revit models.

## Scratchpad
where to attack:
- Element -> model objects
	- reflection may be too expensive to perform *on each object*
	- load their data in as light objects as possible -> ref mirrors of the important bits, monitor diffs
	- monitor by GUID -> versioning, build a git tree of the model/archive at commit.
		- monitor parameters using a ref type struct?
		- must be by value -> because it captures the state of the data at one point in time.
		- record struct