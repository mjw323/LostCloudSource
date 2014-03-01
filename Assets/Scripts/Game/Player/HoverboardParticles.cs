using UnityEngine;
using System;

[Serializable]
public class HoverboardParticles
{
	public ParticleSystem FrontParticles
	{
		get { return frontParticles; }
	}

	public ParticleSystem BackParticles
	{
		get { return backParticles; }
	}

	public ParticleSystem GrindParticles
	{
		get { return grindParticles; }
	}

	public ParticleSystem WaterParticles
	{
		get { return waterParticles; }
	}

	public ParticleSystem SplashParticles
	{
		get { return splashParticles; }
	}

	public HoverboardParticles(Transform hostTransform)
	{
		ParticleSystem[] particleSystems = hostTransform.GetComponentsInChildren<ParticleSystem>();
		frontParticles = particleSystems[0];
		backParticles = particleSystems[1];
		grindParticles = particleSystems[2];
		waterParticles = particleSystems[3];
		splashParticles = particleSystems[4];

		// Why?
		frontParticles.startLifetime = 0.0f;
		backParticles.startLifetime = 0.0f;
		grindParticles.startLifetime = 0.0f;
		waterParticles.startLifetime = 0.0f;
		splashParticles.startLifetime = 0.0f;
	}

	[HideInInspector] private ParticleSystem frontParticles;
	[HideInInspector] private ParticleSystem backParticles;
	[HideInInspector] private ParticleSystem grindParticles;
	[HideInInspector] private ParticleSystem waterParticles;
	[HideInInspector] private ParticleSystem splashParticles;
}