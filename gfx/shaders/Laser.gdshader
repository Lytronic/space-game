shader_type canvas_item;
uniform vec3 colour : source_color;

void fragment() {
	float toMid = abs(0.5 - UV.y) * 2.0f;
	
	COLOR = vec4(colour, 1.0f) * (1.0f - toMid);
}
