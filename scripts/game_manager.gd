extends Node2D

signal game_started
signal game_over

@export var player: Node2D
@export var delivery_manager: Node2D
@export var ui_manager: Node2D

@export var music_player: AudioStreamPlayer
@export var sfx_player: AudioStreamPlayer
@export var pickup_sound: AudioStream
@export var delivery_sound: AudioStream
@export var game_over_sound: AudioStream

enum GameState { MENU, PLAYING, GAME_OVER }

var current_state: GameState = GameState.MENU
var high_score: int = 0
const HIGH_SCORE_KEY = "DeliveryHighScore"

func _ready():
	# Load high score
	high_score = FileAccess.get_file_as_string("user://high_score.save").to_int()
	if high_score == 0:
		high_score = 0
	
	# Connect to delivery events
	if delivery_manager:
		delivery_manager.order_picked_up.connect(_on_order_picked_up)
		delivery_manager.order_delivered.connect(_on_order_delivered)
		delivery_manager.game_over.connect(_on_game_over)

func _on_order_picked_up():
	if player:
		player.pickup_order()
	play_sfx(pickup_sound)

func _on_order_delivered(score):
	if player:
		player.deliver_order()
	play_sfx(delivery_sound)

func _on_game_over():
	current_state = GameState.GAME_OVER
	var final_score = delivery_manager.get_score() if delivery_manager else 0
	if final_score > high_score:
		high_score = final_score
		save_high_score()
	play_sfx(game_over_sound)

func start_game():
	current_state = GameState.PLAYING
	if ui_manager:
		ui_manager.show_hud()
	if player:
		player.reset_player()
	if delivery_manager:
		delivery_manager.reset_deliveries()
	
	if music_player and not music_player.playing:
		music_player.play()
	
	game_started.emit()

func restart_game():
	start_game()

func return_to_menu():
	current_state = GameState.MENU
	if ui_manager:
		ui_manager.show_main_menu(high_score)
	if player:
		player.stop_bike()

func show_main_menu():
	current_state = GameState.MENU
	if ui_manager:
		ui_manager.show_main_menu(high_score)

func play_sfx(clip: AudioStream):
	if sfx_player and clip:
		sfx_player.stream = clip
		sfx_player.play()

func save_high_score():
	var file = FileAccess.open("user://high_score.save", FileAccess.WRITE)
	if file:
		file.store_string(str(high_score))
		file.close()

func get_high_score() -> int:
	return high_score

func quit_game():
	get_tree().quit()