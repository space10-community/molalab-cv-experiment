#version 300 es

uniform sampler2D tex;
in highp vec2 uv;
out lowp vec4 fragcolor;

void main()
{
    fragcolor = texture(tex, uv);
}
