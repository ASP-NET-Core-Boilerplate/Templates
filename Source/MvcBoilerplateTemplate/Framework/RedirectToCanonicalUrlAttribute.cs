﻿namespace $safeprojectname$.Framework
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// To improve Search Engine Optimization SEO, there should only be a single URL for each resource.
    /// Case differences and/or URL's with/without trailing slashes are treated as different URL's by search engines.
    /// This filter redirects all non-canonical URL's based on the settings specified to their canonical equivelant. 
    /// Note: Non-canonical URL's are not generated by this site template, it is usually external sites which are linking
    /// to your site but have changed the URL case or added/removed trailing slashes.
    /// (See http://googlewebmastercentral.blogspot.co.uk/2010/04/to-slash-or-not-to-slash.html).
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class RedirectToCanonicalUrlAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly bool appendTrailingSlash;
        private readonly bool lowercaseUrls;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToCanonicalUrlAttribute" /> class.
        /// </summary>
        /// <param name="appendTrailingSlash">If set to <c>true</c> append trailing slashes, otherwise strip trailing slashes.</param>
        /// <param name="lowercaseUrls">If set to <c>true</c> lowercase all urls.</param>
        public RedirectToCanonicalUrlAttribute(bool appendTrailingSlash, bool lowercaseUrls)
        {
            this.appendTrailingSlash = appendTrailingSlash;
            this.lowercaseUrls = lowercaseUrls;
        }

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether to append trailing slashes.
        /// </summary>
        /// <value>
        /// <c>true</c> if appending trailing slashes; otherwise, strip trailing slashes.
        /// </value>
        public bool AppendTrailingSlash
        {
            get { return this.appendTrailingSlash; }
        }

        /// <summary>
        /// Gets a value indicating whether to lowercase all URL's.
        /// </summary>
        /// <value>
        /// <c>true</c> if lowercasing URL's; otherwise, <c>false</c>.
        /// </value>
        public bool LowercaseUrls
        {
            get { return this.lowercaseUrls; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the HTTP request contains a non-canonical URL using <see cref="TryGetCanonicalUrl"/>, if it doesn't calls the <see cref="HandleNonCanonicalRequest"/> method.
        /// </summary>
        /// <param name="filterContext">An object that encapsulates information that is required in order to use the <see cref="RedirectToCanonicalUrlAttribute"/> attribute.</param>
        /// <exception cref="System.ArgumentNullException">The filterContext parameter is null.</exception>
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            
            // Ignore the Elmah pages.
            if (string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "Elmah", StringComparison.Ordinal))
            {
                return;
            }

            string canonicalUrl;
            if (!TryGetCanonicalUrl(filterContext, out canonicalUrl))
            {
                this.HandleNonCanonicalRequest(filterContext, canonicalUrl);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines whether the specified URl is canonical and if it is not, outputs the canonical URL.
        /// </summary>
        /// <param name="filterContext">An object that encapsulates information that is required in order to use the <see cref="RedirectToCanonicalUrlAttribute" /> attribute.</param>
        /// <param name="canonicalUrl">The canonical URL.</param>
        /// <returns><c>true</c> if the URL is canonical, otherwise <c>false</c>.</returns>
        protected virtual bool TryGetCanonicalUrl(AuthorizationContext filterContext, out string canonicalUrl)
        {
            bool isCanonical = true;

            canonicalUrl = filterContext.HttpContext.Request.Url.ToString();

            bool hasTrailingSlash = canonicalUrl[canonicalUrl.Length - 1] == '/';
            if (this.appendTrailingSlash)
            {
                if (!hasTrailingSlash && !this.HasNoTrailingSlashAttribute(filterContext))
                {
                    canonicalUrl += '/';
                    isCanonical = false;
                }
            }
            else
            {
                if (hasTrailingSlash)
                {
                    canonicalUrl = canonicalUrl.TrimEnd('/');
                    isCanonical = false;
                }
            }

            if (this.lowercaseUrls)
            {
                foreach (char character in canonicalUrl)
                {
                    if (char.IsUpper(character) && !this.HasNoTrailingSlashAttribute(filterContext))
                    {
                        canonicalUrl = canonicalUrl.ToLower();
                        isCanonical = false;
                        break;
                    }
                }
            }

            return isCanonical;
        }

        /// <summary>
        /// Handles HTTP requests for URL's that are not canonical. Performs a 301 Permament Redirect to the canonical URL.
        /// </summary>
        /// <param name="filterContext">An object that encapsulates information that is required in order to use the <see cref="RedirectToCanonicalUrlAttribute" /> attribute.</param>
        /// <param name="canonicalUrl">The canonical URL.</param>
        protected virtual void HandleNonCanonicalRequest(AuthorizationContext filterContext, string canonicalUrl)
        {
            filterContext.Result = new RedirectResult(canonicalUrl, true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether the specified action or its controller has the <see cref="NoTrailingSlashAttribute"/> attribute specified.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        /// <returns></returns>
        private bool HasNoTrailingSlashAttribute(AuthorizationContext filterContext)
        {
            return filterContext.ActionDescriptor.IsDefined(typeof(NoTrailingSlashAttribute), false) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(NoTrailingSlashAttribute), false);
        }

        #endregion
    }
}