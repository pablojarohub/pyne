extends Node2D

@export var move_speed: float = 8.0
@export var wobble_amount: float = 5.0
@export var wobble_speed: float = 10.0

@export var bike_visuals: Node2D
@export var character_visuals: Node2D

var bike: BikeController
var current_wobble: float = 0.0
var wobble_timer: float = 0.0
var original_bike_rotation: Vector2
var original_character_rotation: Vector2

var is_carrying_order: bool = false

func _ready():
	bike = $BikeController
	if bike_visuals:
		original_bike_rotation = bike_visuals.rotation
	if character_visuals:
		original_character_rotation = character_visuals.rotation

func _process(delta):
	handle_input()
	apply_wobble()

func handle_input():
	var horizontal: float = 0.0
	var vertical: float = 0.0

	if Input.is_key_pressed(KEY_A) or Input.is_key_pressed(KEY_LEFT):
		horizontal = -1.0
	elif Input.is_key_pressed(KEY_D) or Input.is_key_pressed(KEY_RIGHT):
		horizontal = 1.0

	if Input.is_key_pressed(KEY_W) or Input.is_key_pressed(KEY_UP):
		vertical = 1.0  # accelerate
	elif Input.is_key_pressed(KEY_S) or Input.is_key_pressed(KEY_DOWN):
		vertical = -1.0  # brake

	bike.set_input(horizontal, vertical)

	# Flip visuals based on direction
	if horizontal != 0.0 and bike_visuals:
		var scale = bike_visuals.scale
		scale.x = sign(horizontal) * abs(scale.x)
		bike_visuals.scale = scale

func apply_wobble():
	if not bike_visuals:
		return

	var speed_ratio = bike.current_speed / bike.max_speed
	if speed_ratio > 0.05:
		wobble_timer += delta * wobble_speed * (1.0 + speed_ratio)
		current_wobble = sin(wobble_timer) * wobble_amount * speed_ratio
	else:
		# Return to neutral when stopped
		current_wobble = lerp(current_wobble, 0.0, delta * 5.0)

	bike_visuals.rotation = current_wobble

	if character_visuals:
		# Character wobbles slightly out of phase for comedic effect
		var char_wobble = sin(wobble_timer * 1.3) * (wobble_amount * 0.5 * speed_ratio)
		character_visuals.rotation = char_wobble

func pickup_order():
	if is_carrying_order:
		return
	is_carrying_order = true
	# TODO: Trigger pickup animation / sound

func deliver_order():
	if not is_carrying_order:
		return
	is_carrying_order = false
	# TODO: Trigger delivery animation / sound

func reset_player():
	is_carrying_order = false
	bike.reset_bike()
	wobble_timer = 0.0
	current_wobble = 0.0
	if bike_visuals:
		bike_visuals.rotation = original_bike_rotation
	if character_visuals:
		character_visuals.rotation = original_character_rotation