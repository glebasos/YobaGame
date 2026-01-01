shader_type canvas_item;

uniform sampler2D dissolve_texture : source_color;
uniform float dissolve_value : hint_range(0, 1);
uniform float burn_size : hint_range(0.0, 1.0, 0.01);
uniform vec4 burn_color : source_color;
// Uniform offset based on a random seed set by main code.
uniform float random_seed;
// Scale for how much to shift the dissolve texture
uniform float offset_strength : hint_range(0.0, 0.1, 0.001);

// A simple pseudo-random function
float rand(vec2 co) {
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void fragment() {
    vec4 main_texture = texture(TEXTURE, UV);

    // Use the random_seed to compute a pseudo-random offset.
    vec2 random_offset = vec2(rand(vec2(random_seed, 0.0)),
                              rand(vec2(random_seed, 1.0)));

    // Apply the offset to the UV coordinates when sampling the dissolve texture.
    vec4 noise_texture = texture(dissolve_texture, UV + offset_strength * random_offset);

    // Calculate burn step and dissolve thresholds.
    float burn_size_step = burn_size * step(0.001, dissolve_value) * step(dissolve_value, 0.999);
    float threshold = smoothstep(noise_texture.x - burn_size_step, noise_texture.x, dissolve_value);
    float border = smoothstep(noise_texture.x, noise_texture.x + burn_size_step, dissolve_value);

    COLOR.a *= threshold;
    COLOR.rgb = mix(burn_color.rgb, main_texture.rgb, border);
}
