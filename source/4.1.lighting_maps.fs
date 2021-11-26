#version 330 core
out vec4 FragColor;

struct Material {
    sampler2D diffuse;
    vec3 specular;    
    float shininess;
}; 

struct Light {
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight {
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    vec3 total_diffuse;
    vec3 total_specular;
};

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    
    float constant;
    float linear;
    float quadratic;
	
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float attenuation;
};

in vec3 FragPos;  
in vec3 Normal;  
in vec2 TexCoords;
  
uniform vec3 viewPos;
uniform Material material;
uniform Light light;
uniform PointLight left_light;
uniform PointLight right_light;
uniform SpotLight spotlight;

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);


void main()
{   
  	
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
        
    vec3 result = CalcPointLight(left_light, norm, FragPos, viewDir);
    result += CalcPointLight(right_light, norm, FragPos, viewDir);
    result += CalcSpotLight(spotlight, norm, FragPos, viewDir);
    FragColor = vec4(result, 1.0);
} 

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = (diff > 0.0) ? pow(max(dot(viewDir, reflectDir), 0.0), material.shininess) : 0.0;
    
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * distance * distance);

    vec3 ambient = attenuation * light.ambient * vec3(texture(material.diffuse, TexCoords));
    vec3 diffuse = attenuation * light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
    vec3 specular = attenuation * light.specular * spec * material.specular;

    return(ambient + diffuse + specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir){

    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = (diff > 0.0) ? pow(max(dot(viewDir, reflectDir), 0.0), material.shininess) : 0.0;
    float cos_theta = dot(lightDir, normalize(-light.direction));

    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * distance * distance);

    if(cos_theta > light.cutOff){       

        vec3 ambient = attenuation * light.ambient * vec3(texture(material.diffuse, TexCoords));
        vec3 diffuse = attenuation * light.diffuse * diff * vec3(texture(material.diffuse, TexCoords));
        vec3 specular = attenuation * light.specular * spec * material.specular;

        return(ambient + diffuse + specular);
    }
    else{
        return (attenuation * light.ambient * vec3(texture(material.diffuse, TexCoords)));
    }

    
}