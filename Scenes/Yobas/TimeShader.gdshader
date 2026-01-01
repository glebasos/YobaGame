shader_type canvas_item;

uniform float time : hint_range(0.0, 1.0);

void fragment() {
    COLOR = vec4(time, 0.5, 0.5, 1.0);
}