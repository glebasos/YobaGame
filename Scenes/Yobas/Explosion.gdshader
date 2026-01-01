shader_type canvas_item;

uniform vec2 fragment_number = vec2(20, 20);
uniform float dissolve_state : hint_range(0.0, 1.0) = 1;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2 pixelFract = fract(UV * fragment_number);
	//COLOR.rgb = vec3(pixelFract, 0);
	float pixelDist = distance(pixelFract, vec2(0.5, 0.5));
	//COLOR.rgb = vec3(pixelDist, 0, 0);
	COLOR = texture(TEXTURE, UV);
	COLOR.a *= step(pixelDist, dissolve_state);
	// Called for every pixel the material is visible on.
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
