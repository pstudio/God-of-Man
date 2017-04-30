using pstudio.GoM.Game.Controllers;
using pstudio.GoM.Game.Models.World;
using UnityEngine;
using Zenject;

namespace pstudio.GoM.Game.Views
{
    public class TileView : MonoBehaviour
    {
        private WorldView _worldView;
        private Land _land;
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer _decoraterSpriteRenderer;
        private ActionController _actionController;

        private bool _animating = false;

        [SerializeField] private float _animationTime = 1f;
        [SerializeField] private float _animationHeight = 1f;

        [Inject]
        public void Construct(WorldView worldView, ActionController actionController)
        {
            _worldView = worldView;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _decoraterSpriteRenderer = transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
            _actionController = actionController;
        }

        public Land Land
        {
            get { return _land; }
            set
            {
                _land = value;
                _spriteRenderer.sprite = _worldView.ViewSettings.LandSprites[(int) value.Type];
                transform.localPosition = new Vector3(value.X, value.Y, 0f);
            }
        }

        public void SetDecorator(Sprite sprite)
        {
            _decoraterSpriteRenderer.sprite = sprite;
        }

        public void Animate()
        {
            if (_animating) return;
            LeanTween.moveZ(gameObject, -_animationHeight, _animationTime).setEase(LeanTweenType.punch).setOnComplete(_ => _animating = false);
            _animating = true;
        }

        private void OnMouseOver()
        {
            if (_actionController.AcceptInput && Input.GetMouseButtonDown(0))
            {
                _actionController.QueueAction(Land.X, Land.Y);
            }

            if (Input.GetMouseButtonDown(1))
                _actionController.Deselect();
            
            //Animate();
            //Debug.Log(Land.Type + " @ [" + Land.X + "," + Land.Y + "] Food: " + Land.Food + ", Animals: " + Land.Animals);
        }

        public class Factory : Factory<TileView> { }
    }

}
