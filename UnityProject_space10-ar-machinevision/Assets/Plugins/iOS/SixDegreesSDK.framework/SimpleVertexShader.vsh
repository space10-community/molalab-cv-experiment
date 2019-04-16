#version 300 es

in vec4 position;
in vec2 texcoord;
out vec2 uv;

void main()
{
    gl_Position = position;
    uv = texcoord;
}
