@tool
extends Control

# Radius of the circle
@export var radius: float = 100.0:
	get:
		return radius
	set(value):
		radius = value
		arrange_radial()

# Offset angle in degrees (optional)
@export var start_angle: float = 0.0:
	get:
		return start_angle
	set(value):
		start_angle = value
		arrange_radial()

# Automatically update in the editor
func _notification(what):
	if what == NOTIFICATION_ENTER_TREE or what == NOTIFICATION_SCENE_INSTANTIATED or what == NOTIFICATION_CHILD_ORDER_CHANGED:
		arrange_radial()

func arrange_radial():
	# Ensure this works only in the editor or during runtime
	if not Engine.is_editor_hint() and not is_inside_tree():
		return

	# Get the center of this Control node
	var center = size / 2.0
	# Get all child nodes
	var children = get_children()
	var count = children.size()

	if count == 0:
		return

	# Calculate the angle step based on the number of elements
	var angle_step = 360.0 / count

	# Position each child node
	for i in range(count):
		var angle_deg = start_angle + (i * angle_step)
		var angle_rad = deg_to_rad(angle_deg)

		# Calculate the position on the circle
		var x = center.x + radius * cos(angle_rad)
		var y = center.y + radius * sin(angle_rad)

		# Set the child's position (for Control nodes)
		if children[i] is Control:
			var child = children[i] as Control
			child.position = Vector2(x, y) - (child.size / 2.0)
