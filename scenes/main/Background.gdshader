shader_type canvas_item;

// Voronoi noise and random implementation adapted from https://godotshaders.com/snippet/voronoi/

// pseudo-random functions
// float literal values are just random values3

// this one raises its output to the power of 90 in order to exaggerate values
// otherwise it would just be normal voronoi noise without the star-like appearance
vec2 random(vec2 uv) {
	return vec2(pow(fract(sin(dot(uv.xy,
		vec2(9.234789, 1.98976))) * 467.321), 90.0));
}

float randomf(vec2 uv) {
	return pow(fract(sin(dot(uv.xy,
		vec2(9.234789, 1.98976))) * 467.321), 1.0);
}

// Voronoi noise function
vec2 voronoi(vec2 uv, float columns, float rows) {
	
	vec2 index_uv = floor(vec2(uv.x * columns, uv.y * rows));
	vec2 fract_uv = fract(vec2(uv.x * columns, uv.y * rows));
	
	float minimum_dist = 1.0;  
	vec2 minimum_point;

	for (int y= -1; y <= 1; y++) {
		for (int x= -1; x <= 1; x++) {
			vec2 neighbor = vec2(float(x),float(y));
			vec2 point = random(index_uv + neighbor);

			vec2 diff = neighbor + point - fract_uv;
			float dist = length(diff);
			
			if(dist < minimum_dist) {
				minimum_dist = dist;
				minimum_point = point;
			}
		}
	}
	return minimum_point;
}

// end of Voronoi implementation

// function to convert a star temperature to rgb values
// own implementation of this algorithm: https://tannerhelland.com/2012/09/18/convert-temperature-rgb-algorithm-code.html
vec3 kelvin2rgb(float temp) {
	temp /= 100.0;
	vec3 ret = vec3(0.0);
	
	// red
	if (temp <= 66.0) {
		ret.r = 255.0;
	} else {
		ret.r = temp - 60.0;
		ret.r = 329.698727446 * pow(ret.r, -0.1332047592);
	}
	
	// green
	if (temp <= 66.0) {
		ret.g = temp;
		ret.g = 99.4708025861 * log(ret.g) - 161.1195681661;
	} else {
		ret.g = temp - 60.0;
		ret.g = 288.1221695283 * pow(ret.g, -0.0755148492);
	}
	
	// blue
	if (temp >= 66.0) {
		ret.b = 255.0;
	} else {
		if (temp <= 19.0) {
			ret.b = 0.0;
		} else {
			ret.b = temp - 10.0;
			ret.b = 138.5177312231 * log(ret.b) - 305.0447927307;
		}
	}
	
	// convert to 0..1 range
	ret /= vec3(255.0);
	
	return ret;
}

void fragment() {
	vec2 v = voronoi(SCREEN_UV, 500.0, 500.0);
	
	// generate a random temperature and transform it to rgb
	vec3 kcol = kelvin2rgb(clamp(randomf(SCREEN_UV) * 50000.0, 1000.0, 40000.0));
	
	// add even more randomness and a sine wave to make some stars twinkle
	COLOR = vec4(kcol * v.r * (sin(TIME * length(random(SCREEN_UV) * 5.0)) + 1.0), 1.0);
}
