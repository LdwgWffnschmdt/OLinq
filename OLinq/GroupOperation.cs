﻿using System.Collections;
using System.Collections.Specialized;
using System.Linq.Expressions;

namespace OLinq
{

    abstract class GroupOperation<T> : Operation<T>
    {

        IOperation<IEnumerable> source;

        public GroupOperation(OperationContext context, MethodCallExpression expression)
            : base(context, expression)
        {
            if (expression.Arguments.Count >= 1)
            {
                source = OperationFactory.FromExpression<IEnumerable>(context, expression.Arguments[0]);
                source.ValueChanged += source_ValueChanged;
            }
        }

        void source_ValueChanged(object sender, ValueChangedEventArgs args)
        {
            var oldValue = args.OldValue as INotifyCollectionChanged;
            if (oldValue != null)
                oldValue.CollectionChanged -= source_CollectionChanged;

            var newValue = args.NewValue as INotifyCollectionChanged;
            if (newValue != null)
                newValue.CollectionChanged += source_CollectionChanged;

            OnSourceChanged((IEnumerable)args.OldValue, (IEnumerable)args.NewValue);
        }

        void source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            OnSourceCollectionChanged(args);
        }

        protected IEnumerable Source
        {
            get { return source.Value; }
        }

        public override void Load()
        {
            if (source != null)
                source.Load();
            base.Load();
        }

        public override void Dispose()
        {
            if (source != null)
            {
                var sourceValue = source.Value as INotifyCollectionChanged;
                source.ValueChanged -= source_ValueChanged;
                source.Dispose();
                if (sourceValue != null)
                    sourceValue.CollectionChanged -= source_CollectionChanged;
            }

            base.Dispose();
        }

        /// <summary>
        /// Invoked when the value of an argument is changed.
        /// </summary>
        protected abstract void OnSourceChanged(IEnumerable oldValue, IEnumerable newValue);

        /// <summary>
        /// Invoked when a collection change notification is received from the value of an argument.
        /// </summary>
        /// <param name="args"></param>
        protected abstract void OnSourceCollectionChanged(NotifyCollectionChangedEventArgs args);

    }

}
