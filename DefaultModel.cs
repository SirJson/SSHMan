using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace SSHMan
{


    /// <summary>
    /// Implements a base class for all viewmodel classes
    /// that implements <seealso cref="INotifyPropertyChanged"/> interface for binding.
    /// </summary>
    public class DefaultModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Standard implementation of <seealso cref="INotifyPropertyChanged"/>.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Tell bound controls (via WPF binding) to refresh their display.
        /// 
        /// Sample call: this.NotifyPropertyChanged(() => this.IsSelected);
        /// where 'this' is derived from <seealso cref="DefaultModel"/>
        /// and IsSelected is a property.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;
            var memberExpression = lambda.Body is UnaryExpression unaryExpression ? (MemberExpression)unaryExpression.Operand : (MemberExpression)lambda.Body;
            this.RaisePropertyChanged(memberExpression.Member.Name);
        }

        /// <summary>
        /// Tell bound controls (via WPF binding) to refresh their display.
        /// Standard implementation through <seealso cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void RaisePropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
