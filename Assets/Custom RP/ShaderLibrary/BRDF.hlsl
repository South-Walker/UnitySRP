#ifndef CUSTOM_BRDF_INCLUDED
#define CUSTOM_BRDF_INCLUDED
struct BRDF
{
	float3 diffuse;
	float3 specular;
	float roughness;
};
#define MIN_REFLECTIVITY 0.04
//��ȥ���淴��ʣ��������ı���
//���Ǵ���������Ҫ�ٿ�ȥMIN_REFLECTIVITY
float OneMinusReflectivity(float metallic)
{
	float range = 1.0 - MIN_REFLECTIVITY;
	return range - metallic * range;
}
BRDF GetBRDF(Surface surface, bool applyAlphaToDiffuse = false)
{
	BRDF brdf;
	float oneMinusReflectivity = OneMinusReflectivity(surface.metallic);
	//���Ƚ�MIN_REFLECTIVITY�����Ĺ�ȫ�����ھ��淴��
	//�ٽ�ʣ��(1-MIN_REFLECTIVITY)���ְ�metallic����Ϊ���淴����������
	//���ݶ��壬��ƽ�б��淨��������ʱ�������Ϊsurface.color
	brdf.diffuse = surface.color * oneMinusReflectivity;
	if (applyAlphaToDiffuse)
	{
		//����alphaֵ��Ԥ˥������Ϊ��blend�趨�в�˥��Src,Ŀ���Ǳ�֤���淴�䲻��˥�������ǲ������οյ�����
		brdf.diffuse *= surface.alpha;
	}

	brdf.specular = lerp(MIN_REFLECTIVITY, surface.color, surface.metallic);
	float perceptualRoughness =
		PerceptualSmoothnessToPerceptualRoughness(surface.smoothness);
	brdf.roughness = PerceptualRoughnessToRoughness(perceptualRoughness);
	return brdf;
}
#endif