extends Node2D

@export var max_speed: float = 12.0
@export var acceleration: float = 15.0
@export var deceleration: float = 8.0
@export var brake_force: float = 25.0
@export var turn_speed: float = 10.0
@export var linear_drag: float = 2.0

var rb: RigidBody2D
var horizontal_input: float = 0.0
var vertical_input: float = 0.0

var current_speed: float = 0.0

func _ready():
	rb = $RigidBody2D
	if not rb:
		push_error("BikeController: RigidBody2D node not found!")
		return
	
	rb.gravity_scale = 0.0
	rb.linear_damping = linear_drag
	rb.freeze_rotation = true

func _physics_process(delta):
	apply_movement()
	current_speed = rb.linear_velocity.length()

func set_input(horizontal: float, vertical: float):
	horizontal_input = horizontal
	vertical_input = vertical

func apply_movement():
	var desired_velocity = Vector2.ZERO

	# Horizontal movement
	if abs(horizontal_input) > 0.01:
		desired_velocity.x = horizontal_input * max_speed

	# Accelerate / Brake logic
	if vertical_input > 0.01:
		# Accelerating – increase max speed temporarily (boost feel)
		desired_velocity.x = horizontal_input * (max_speed * 1.2)
	elif vertical_input < -0.01:
		# Braking – apply strong deceleration
		var brake_velocity = lerp(rb.linear_velocity, Vector2.ZERO, brake_force * delta)
		rb.linear_velocity = brake_velocity
		return  # Skip normal force application when braking

	# Smoothly interpolate current velocity toward desired velocity
	var new_velocity = lerp(rb.linear_velocity, desired_velocity, acceleration * delta)

	# Apply deceleration when no horizontal input
	if abs(horizontal_input) < 0.01 and vertical_input > -0.01:
		new_velocity = lerp(rb.linear_velocity, Vector2.ZERO, deceleration * delta)

	rb.linear_velocity = new_velocity

func stop_bike():
	rb.linear_velocity = Vector2.ZERO

func reset_bike():
	rb.linear_velocity = Vector2.ZERO
	rb.angular_velocity = 0.0
	position = Vector2.ZERO