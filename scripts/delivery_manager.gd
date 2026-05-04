extends Node2D

signal order_picked_up
signal order_delivered(score: int)
signal game_over

enum DeliveryState { IDLE, GOING_TO_PICKUP, CARRYING_ORDER, DELIVERED }

@export var pickup_points: Array[Node2D] = []
@export var delivery_points: Array[Node2D] = []
@export var ui_manager: Node2D

@export var base_delivery_score: int = 100
@export var fast_delivery_threshold: float = 15.0
@export var fast_delivery_multiplier: float = 1.5

@export var time_reduction_per_delivery: float = 1.0
@export var min_delivery_time: float = 8.0

var current_state: DeliveryState = DeliveryState.IDLE
var current_pickup: Node2D
var current_delivery: Node2D
var order_start_time: float
var current_time_limit: float
var total_deliveries: int = 0
var current_score: int = 0

func _ready():
	current_time_limit = fast_delivery_threshold
	start_new_delivery()

func _process(delta):
	if current_state == DeliveryState.CARRYING_ORDER:
		var elapsed = Time.get_ticks_msec() / 1000.0 - order_start_time
		var remaining = max(0.0, current_time_limit - elapsed)
		
		if ui_manager:
			ui_manager.update_timer(remaining, current_time_limit)
		
		if remaining <= 0.0:
			trigger_game_over("Time's up! The food got cold.")

func start_new_delivery():
	if pickup_points.size() == 0 or delivery_points.size() == 0:
		push_error("[DeliveryManager] No pickup or delivery points assigned!")
		return

	current_state = DeliveryState.GOING_TO_PICKUP
	current_pickup = get_random_point(pickup_points)
	current_delivery = null

	if ui_manager:
		ui_manager.show_task("Go to " + get_location_name(current_pickup) + " to pick up the order!")
		ui_manager.show_arrow(current_pickup.global_position)

func pickup_order():
	if current_state != DeliveryState.GOING_TO_PICKUP:
		return

	current_state = DeliveryState.CARRYING_ORDER
	order_start_time = Time.get_ticks_msec() / 1000.0

	# Select a delivery point different from pickup
	while true:
		current_delivery = get_random_point(delivery_points)
		if current_delivery != current_pickup:
			break

	if ui_manager:
		ui_manager.show_task("Deliver to " + get_location_name(current_delivery) + "!")
		ui_manager.show_arrow(current_delivery.global_position)
	
	order_picked_up.emit()

func deliver_order():
	if current_state != DeliveryState.CARRYING_ORDER:
		return

	var elapsed = Time.get_ticks_msec() / 1000.0 - order_start_time
	var is_fast = elapsed <= current_time_limit
	var score = calculate_score(is_fast)
	current_score += score
	total_deliveries += 1

	# Increase difficulty
	current_time_limit = max(min_delivery_time, fast_delivery_threshold - (total_deliveries * time_reduction_per_delivery))

	current_state = DeliveryState.DELIVERED
	
	if ui_manager:
		ui_manager.update_score(current_score)
		ui_manager.show_task("Delivery complete! +" + str(score) + " points")
	
	order_delivered.emit(score)

	# Start next delivery after a short delay
	await get_tree().create_timer(1.5).timeout
	start_new_delivery()

func get_current_target() -> Node2D:
	if current_state == DeliveryState.GOING_TO_PICKUP:
		return current_pickup
	elif current_state == DeliveryState.CARRYING_ORDER:
		return current_delivery
	return null

func get_score() -> int:
	return current_score

func get_total_deliveries() -> int:
	return total_deliveries

func calculate_score(is_fast: bool) -> int:
	var score = base_delivery_score
	if is_fast:
		score = int(score * fast_delivery_multiplier)
	return score

func get_random_point(points: Array[Node2D]) -> Node2D:
	if points.size() == 0:
		return null
	return points[randi() % points.size()]

func get_location_name(point: Node2D) -> String:
	if point == null:
		return "Unknown"
	
	# Use custom name or node name
	var location_name = point.get("display_name")
	if location_name:
		return location_name
	return point.name

func trigger_game_over(reason: String):
	current_state = DeliveryState.IDLE
	if ui_manager:
		ui_manager.show_game_over(current_score, total_deliveries, reason)
	game_over.emit()

func reset_deliveries():
	current_state = DeliveryState.IDLE
	current_score = 0
	total_deliveries = 0
	current_time_limit = fast_delivery_threshold
	current_pickup = null
	current_delivery = null
	start_new_delivery()