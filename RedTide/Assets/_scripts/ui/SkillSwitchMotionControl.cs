using UnityEngine;

public class SkillSwitchMotionControl : MonoBehaviour
{
	public bool Current { get; set; }

	private float _width = 99;
	private float _duration = 0.5f;
	private RectTransform _rectTransform;
	private Vector3 _currentPosX = Vector3.zero;
	private Vector3 _targetPosX = Vector3.zero;

	private bool _active;
	private float _acc;

	// Use this for initialization
	void Start ()
	{
		_rectTransform = GetComponent<RectTransform>();
		_active = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (_active)
		{
			_acc += Time.deltaTime;
			if (_acc < _duration)
			{
				float ratio = _acc / _duration;
				Vector3 pos = Vector3.Lerp(_currentPosX, _targetPosX, ratio);
				_rectTransform.anchoredPosition3D = pos;
			}
			else
			{
				_acc = _duration;
				_rectTransform.anchoredPosition3D = _targetPosX;
				_active = false;
			}
		}
	}

	public void Switch(bool left)
	{
		if (left)
		{
			if (Current)
			{
				_rectTransform.anchoredPosition3D.Set(0, 0, 0);
				_currentPosX = Vector3.zero;
				_targetPosX = new Vector3(-_width, 0, 0);
			}
			else
			{
				_rectTransform.anchoredPosition3D.Set(_width, 0, 0);
				_currentPosX = new Vector3(_width, 0, 0);
				_targetPosX = Vector3.zero;
			}
		}
		else
		{
			if (Current)
			{
				_rectTransform.anchoredPosition3D.Set(0, 0, 0);
				_currentPosX = Vector3.zero;
				_targetPosX = new Vector3(_width, 0, 0);
			}
			else
			{
				_rectTransform.anchoredPosition3D.Set(-_width, 0, 0);
				_currentPosX = new Vector3(-_width, 0, 0);
				_targetPosX = Vector3.zero;
			}
		}
		_active = true;
		_acc = 0;
		Current = !Current;
	}
}
