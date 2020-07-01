#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED
struct BRDF
{
	float3 diffuse;
	float3 specular;
	float roughness;
};
#define MIN_REFLECTIVITY 0.04
//除去镜面反射剩下漫反射的比例
//但是从漫反射中要再扣去MIN_REFLECTIVITY
float OneMinusReflectivity(float metallic)
{
	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range;
}
BRDF GetBRDF(Surface surface, bool applyAlphaToDiffuse = false)
{
	BRDF brdf;
	float oneMinusReflectivity = OneMinusReflectivity(surface.metallic);
	//首先将MIN_REFLECTIVITY比例的光全部用于镜面反射
	//再将剩下(1-MIN_REFLECTIVITY)部分按metallic分配为镜面反射与漫反射
	//根据定义，光平行表面法向量入射时三者相加为surface.color
	brdf.diffuse = surface.color * oneMinusReflectivity;
	if (applyAlphaToDiffuse)
	{
		//基于alpha值的预衰减，因为再blend设定中不衰减Src,目的是保证镜面反射不被衰减，这是玻璃与镂空的区别
		brdf.diffuse *= surface.alpha;
	}

	brdf.specular = lerp(MIN_REFLECTIVITY, surface.color, surface.metallic);
	float perceptualRoughness =
		PerceptualSmoothnessToPerceptualRoughness(surface.smoothness);
	brdf.roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
	return brdf;
}
#endif