shader_type canvas_item;

uniform float intensity;
uniform vec3 colour : source_color;

void fragment() {
	vec2 center_offset = vec2(0.5) - SCREEN_UV;
	// Chebyshev distance
	float distance = max(abs(center_offset.x), abs(center_offset.y));
	float alpha = clamp(distance - 0.25, 0.0f, 1.0f) * intensity;
	COLOR = vec4(colour, alpha);

}
