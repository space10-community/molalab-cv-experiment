#version 300 es

uniform sampler2D texY;
uniform sampler2D texUV;
in highp vec2 uv;
out lowp vec4 fragcolor;

void main()
{
    mediump vec3 yuv;
    lowp vec3 rgb;
    
    yuv.x = texture(texY, uv).r;
    yuv.yz = texture(texUV, uv).ra - vec2(0.5, 0.5);
    
    // BT.601, which is the standard for SDTV is provided as a reference
    /*
    rgb = mat3(    1,       1,     1,
                   0, -.34413, 1.772,
               1.402, -.71414,     0) * yuv;
     */
    
    // Using BT.709 which is the standard for HDTV
    rgb = mat3(      1,       1,      1,
                     0, -.18732, 1.8556,
               1.57481, -.46813,      0) * yuv;
    
    fragcolor = vec4(rgb, 1);
}

