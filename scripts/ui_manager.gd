extends Node2D

signal start_button_pressed
signal restart_button_pressed
signal menu_button_pressed
signal quit_button_pressed

@export var main_menu_panel: Control
@export var hud_panel: Control
@export var game_over_panel: Control

@export var score_text: Label
@export var task_text: Label
@export var timer_text: Label
@export var high_score_text: Label
@export var timer_fill_image: TextureRect

@export var final_score_text: Label
@export var deliveries_text: Label
@export var game_over_reason_text: Label
@export var game_over_high_score_text: Label

@export var arrow_rect: Control
@export var player_transform: Node2D
@export var main_camera: Camera2D

@export var task_text_display_time: float = 2.0

var task_text_timer: float = 0.0

func _ready():
	if not main_camera:
		main_camera = get_tree().get_first_node_in_group("main_camera")

func _process(delta):
	update_arrow_indicator()

	# Hide task text after a delay
	if task_text_timer > 0.0:
		task_text_timer -= delta
		if task_text_timer <= 0.0 and task_text:
			task_text.visible = false

func show_main_menu(high_score: int):
	if main_menu_panel:
		main_menu_panel.visible = true
	if hud_panel:
		hud_panel.visible = false
	if game_over_panel:
		game_over_panel.visible = false
	
	if high_score_text:
		high_score_text.text = "High Score: " + str(high_score)

func show_hud():
	if main_menu_panel:
		main_menu_panel.visible = false
	if hud_panel:
		hud_panel.visible = true
	if game_over_panel:
		game_over_panel.visible = false

func show_game_over(score: int, deliveries: int, reason: String):
	if main_menu_panel:
		main_menu_panel.visible = false
	if hud_panel:
		hud_panel.visible = false
	if game_over_panel:
		game_over_panel.visible = true
	
	if final_score_text:
		final_score_text.text = "Score: " + str(score)
	if deliveries_text:
		deliveries_text.text = "Deliveries: " + str(deliveries)
	if game_over_reason_text:
		game_over_reason_text.text = reason

func update_score(score: int):
	if score_text:
		score_text.text = "Score: " + str(score)

func show_task(message: String):
	if task_text:
		task_text.text = message
		task_text.visible = true
		task_text_timer = task_text_display_time

func update_timer(remaining: float, max_time: float):
	if timer_text:
		timer_text.text = str(ceil(remaining)) + "s"

	if timer_fill_image:
		var ratio = clamp(remaining / max_time, 0.0, 1.0)
		timer_fill_image.material.set_shader_parameter("fill_amount", ratio)
		# Change color based on urgency
		if ratio > 0.3:
			timer_fill_image.modulate = Color.GREEN
		else:
			timer_fill_image.modulate = Color.RED

func update_arrow_indicator():
	if not arrow_rect or not player_transform or not main_camera:
		return

	# Find current target
	var target_pos = get_current_target_position()
	if not target_pos:
		arrow_rect.visible = false
		return

	var target = target_pos
	var screen_pos = main_camera.get_camera_screen_position(target)
	var center = get_viewport_rect().size / 2.0

	# If target is on screen, hide arrow
	if screen_pos.x >= 0 and screen_pos.x <= get_viewport_rect().size.x and \
	   screen_pos.y >= 0 and screen_pos.y <= get_viewport_rect().size.y:
		arrow_rect.visible = false
		return

	arrow_rect.visible = true

	# Calculate direction from center to target
	var dir = screen_pos - center
	var angle = dir.angle() - PI / 2  # Convert to degrees and adjust for arrow pointing up
	arrow_rect.rotation = angle

	# Position arrow at edge of screen
	var edge_pos = center + dir.normalized() * 120.0
	arrow_rect.position = edge_pos

func get_current_target_position() -> Vector2:
	var dm = GameManager.delivery_manager
	if not dm:
		return Vector2.ZERO
	
	var target = dm.get_current_target()
	if target:
		return target.global_position
	return Vector2.ZERO

func on_start_button_clicked():
	start_button_pressed.emit()

func on_restart_button_clicked():
	restart_button_pressed.emit()

func on_menu_button_clicked():
	menu_button_pressed.emit()

func on_quit_button_clicked():
	quit_button_pressed.emit()