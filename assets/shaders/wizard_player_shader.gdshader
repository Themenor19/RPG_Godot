shader_type canvas_item;
render_mode blend_mix;

instance uniform vec4 MaskColor : source_color;
instance uniform float FloatParameter : hint_range(0.001, 0.003);
instance uniform vec4 GlowColor : source_color;
instance uniform vec4 CloakMaskColor : source_color;
instance uniform vec4 CloakShadingMaskColor : source_color;
instance uniform vec4 NewCloakColor : source_color;
instance uniform vec4 NewCloakShadingColor : source_color;



void fragment() {
	
// Input:3
	vec4 n_out3p0 = COLOR;
	
	  // Check if pixel matches the cloak color (with tolerance)
    vec3 cloakDiff = abs(n_out3p0.rgb - CloakMaskColor.rgb);
	vec3 cloakShadeDiff = abs(n_out3p0.rgb - CloakShadingMaskColor.rgb);
    bool isCloakPixel = all(lessThan(cloakDiff, vec3(FloatParameter)));
	bool isCloakShadingPixel = all(lessThan(cloakShadeDiff, vec3(FloatParameter)));

    if (isCloakPixel) {
        COLOR.rgb = NewCloakColor.rgb;
    } else if(isCloakShadingPixel){
		COLOR.rgb = NewCloakShadingColor.rgb;
	} else {
	
// ColorParameter:4
	vec4 n_out4p0 = MaskColor;
// ColorOp:2
	vec3 n_out2p0 = abs(vec3(n_out3p0.xyz) - vec3(n_out4p0.xyz));
// FloatParameter:6
	float n_out6p0 = FloatParameter;
	vec3 n_out5p0;
// If:5
	float n_in5p1 = 0.00000;
	vec3 n_in5p3 = vec3(1.00000, 1.00000, 1.00000);
	vec3 n_in5p4 = vec3(0.00000, 0.00000, 0.00000);
	vec3 n_in5p5 = vec3(0.00000, 0.00000, 0.00000);
	if(abs(n_out2p0.x - n_in5p1) < n_out6p0)
	{
		n_out5p0 = n_in5p3;
	}
	else if(n_out2p0.x < n_in5p1)
	{
		n_out5p0 = n_in5p5;
	}
	else
	{
		n_out5p0 = n_in5p4;
	}
// ColorParameter:8
	vec4 n_out8p0 = GlowColor;


	vec3 n_out7p0;
// Switch:7
	n_out7p0 = mix(vec3(n_out3p0.xyz), vec3(n_out8p0.xyz), float(all(bvec3(n_out5p0))));
	


// Output:0
	COLOR.rgb = n_out7p0;

}
}
